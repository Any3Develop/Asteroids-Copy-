using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Services.EntityService.Factory
{
    public class SceneEnityFactory<T> : IInitializable, ISceneEntityFactory<T> where T : ISceneEntity
    {
        private readonly IInstantiator instantiator;
        private Dictionary<string, Object> prefabs;
        public string ResourcePath { get; }

        public SceneEnityFactory(string path, IInstantiator instantiator)
        {
            ResourcePath = path;
            this.instantiator = instantiator;
        }
        
        public void Initialize()
        {
            prefabs = Resources
                .LoadAll<Object>(ResourcePath)
                .ToDictionary(k=> k.name, v => v);
        }

        public TEntity Create<TEntity>(string name = null, Transform parent = null, params object[] args)
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(TEntity).Name;
            
            var entity = Create(name, parent, args);
            if (entity is TEntity tEntity)
                return tEntity;
            
            throw new InvalidCastException($"Cant cast entity to : {nameof(TEntity)}, but object created : {entity?.GetType().Name}");
        }

        public T Create(string name, Transform parent = null, params object[] args)
        {
            if (string.IsNullOrEmpty(name))
                throw new NullReferenceException($"Cant create scene entity. Target typeName is empty");
                    
            if (!prefabs.ContainsKey(name))
                throw new NullReferenceException($"Cant create scene entity. Target type : {name} is missing");

            var prefab = prefabs[name];
            if (prefab == null)
                throw new InvalidOperationException($"Key : {name} exist but prefab is missing!");

            var entity = instantiator.InstantiatePrefabForComponent<T>(prefab, parent, args);
            entity.Container.name = name;
            return entity;
        }
    }
}