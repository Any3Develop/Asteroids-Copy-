using System;

namespace Services.EntityService.Pool
{
    public interface IEntityPool<out T> : IEntityPool
    {
        TEntity Spawn<TEntity>(string name = null, params object[] args) where TEntity : IEntityPoolable;
        new T Spawn(string name, params object[] args);
    }

    public interface IEntityPool : IEntity , IDisposable
    {
        event Action<IEntityPoolable> OnSpawned;
        event Action<IEntityPoolable> OnDespawned;
        IEntityPoolable[] GetActiveEntities(string entityId = null);
        IEntityPoolable Spawn(string name, params object[] args);
        void Despawn(IEntityPoolable entity);
        void DespawnLate(IEntityPoolable entity, Action onComplete);
    }
}