using Services.EntityService.Factory;
using UnityEngine;

namespace Services.EntityService.Pool
{
    public class SceneEntityPool<T> : EntityPoolBase, IEntityPool<T> where T : ISceneEntity, IEntityPoolable
    {
        private readonly ISceneEntityFactory<T> factory;
        private readonly Transform poolContainer;
        public override string Id { get; }
        
        public SceneEntityPool(string id, ISceneEntityFactory<T> factory, IEntityPoolCleaner cleaner) : base(cleaner)
        {
            Id = id;
            this.factory = factory;
            poolContainer = new GameObject(id).transform;
        }
        
        public new T Spawn(string name, params object[] args)
        {
            return (T) base.Spawn(name, args);
        }

        protected override TEntity SpawnInernal<TEntity>(string name = null, params object[] args)
        {
            return factory.Create<TEntity>(name, poolContainer, args);
        }

        protected override IEntityPoolable SpawnInernal(string name, params object[] args)
        {
            return factory.Create(name, poolContainer, args);
        }
    }
}