using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.EntityService.Pool
{
    public abstract class EntityPoolBase : IEntityPool
    {
        protected readonly List<IEntityPoolable> Pool;
        protected readonly List<IEntityPoolable> Active;
        
        public abstract string Id { get; }
        public event Action<IEntityPoolable> OnSpawned;
        public event Action<IEntityPoolable> OnDespawned;


        protected EntityPoolBase(IEntityPoolCleaner cleaner)
        {
            Pool = new List<IEntityPoolable>();
            Active = new List<IEntityPoolable>();
            cleaner.AddPool(this);
        }

        public IEntityPoolable[] GetActiveEntities(string entityId = null)
        {
            return !string.IsNullOrEmpty(entityId) 
            ? Active.Where(x => x.Id == entityId).ToArray()
            : Active.ToArray();

        }

        public TEntity Spawn<TEntity>(string name = null, params object[] args) where TEntity : IEntityPoolable
        {
            var poolable = string.IsNullOrEmpty(name)
                ? Pool.OfType<TEntity>().FirstOrDefault()
                : Pool.Where(x => x.Name == name).OfType<TEntity>().FirstOrDefault();
            if (poolable == null)
            {
                poolable = SpawnInernal<TEntity>(name, args);
                Active.Add(poolable);
                poolable.Spawn();
                OnSpawned?.Invoke(poolable);
                return poolable;
            }

            Pool.Remove(poolable);
            Active.Add(poolable);
            poolable.Spawn();
            OnSpawned?.Invoke(poolable);
            return poolable;
        }

        public IEntityPoolable Spawn(string name, params object[] args)
        {
            var poolable = Pool.FirstOrDefault(x=> x.Name == name);
            if (poolable == null)
            {
                poolable = SpawnInernal(name, args);
                Active.Add(poolable);
                poolable.Spawn();
                OnSpawned?.Invoke(poolable);
                return poolable;
            }

            Pool.Remove(poolable);
            Active.Add(poolable);
            poolable.Spawn();
            OnSpawned?.Invoke(poolable);
            return poolable;
        }

        public void Despawn(IEntityPoolable entity)
        {
            if (entity == null)
                return;

            if (!Active.Contains(entity))
                return;
            
            Active.Remove(entity);
            OnDespawned?.Invoke(entity);
            entity.Despawn();
            Pool.Add(entity);
        }

        public async void DespawnLate(IEntityPoolable entity, Action onComplete)
        {
            await Task.Yield();
            Despawn(entity);
            onComplete?.Invoke();
        }

        public void Dispose()
        {
            foreach (var enity in Active.ToArray())
                Despawn(enity);
            Pool.ForEach(x => x?.Dispose());
            Active.ForEach(x => x?.Dispose());
            Pool.Clear();
            Active.Clear();
        }
        
        protected abstract TEntity SpawnInernal<TEntity>(string name = null, params object[] args) where TEntity : IEntityPoolable;
        protected abstract IEntityPoolable SpawnInernal(string name, params object[] args);
    }
}