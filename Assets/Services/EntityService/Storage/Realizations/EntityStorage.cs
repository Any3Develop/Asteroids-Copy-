using System;
using System.Collections.Generic;
using System.Linq;
using Services.Extensions;
using Services.LoggerService;

namespace Services.EntityService.Storage
{
    public class EntityStorage<T> : IDisposable, IEntityStorage<T> where T : IEntity
    {
        private readonly Dictionary<string, List<T>> storage;
        public event Action<T> OnAdded;
        public event Action<T> OnRemoved;

        public EntityStorage()
        {
            storage = new Dictionary<string, List<T>>();
        }

        public T Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                DefaultLogger.Error("Id is empty");
                return default;
            }

            return storage.ContainsKey(id)
                ? storage[id].FirstOrDefault()
                : default;
        }

        public IEnumerable<T> GetAll(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                DefaultLogger.Error("Id is empty");
                return default;
            }

            return storage.ContainsKey(id)
                ? storage[id].ToArray()
                : Array.Empty<T>();
        }

        public IEnumerable<T> Get()
        {
            return storage.Values.SelectMany(x => x).ToArray();
        }

        public IEnumerable<T> GetExcept(params string[] ids)
        {
            return !ids.IsNullOrEmpty()
                ? storage.Where(x => !ids.Contains(x.Key)).SelectMany(x => x.Value).ToArray()
                : Get();
        }

        public bool TryGet(string id, out T result)
        {
            result = Get(id);
            return result != null;
        }

        public void Add(T entity)
        {
            if (entity == null)
                return;
            
            if (string.IsNullOrEmpty(entity.Id))
            {
                DefaultLogger.Error("Id is empty");
                return;
            }

            if (!storage.ContainsKey(entity.Id))
                storage.Add(entity.Id, new List<T>());

            if (storage[entity.Id].Contains(entity))
                return;

            storage[entity.Id].Add(entity);
            OnAdded?.Invoke(entity);
        }

        public void Add(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public bool Contains(string id)
        {
            return storage.ContainsKey(id);
        }

        public bool Contains(T entity)
        {
            if (entity == null)
                return false;
            
            if (string.IsNullOrEmpty(entity.Id))
            {
                DefaultLogger.Error("Id is empty");
                return false;
            }
            
            return storage.ContainsKey(entity.Id)
                && storage[entity.Id].Contains(entity);
        }

        public void Remove(T entity)
        {
            if (entity == null)
                return;
            
            if (string.IsNullOrEmpty(entity.Id))
            {
                var (key, value) = storage.FirstOrDefault(x => x.Value.Contains(entity));
                if (string.IsNullOrEmpty(key))
                    return;
                
                value.Remove(entity);
                if (value.Count == 0)
                    storage.Remove(key);
                
                OnRemoved?.Invoke(entity);
                return;
            }

            if (!storage.ContainsKey(entity.Id))
                return;

            storage[entity.Id].Remove(entity);
            if (storage[entity.Id].Count == 0)
                storage.Remove(entity.Id);
            
            OnRemoved?.Invoke(entity);
        }

        public void Remove(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                DefaultLogger.Error("Id is empty");
                return;
            }
            
            if (!storage.ContainsKey(id))
                return;

            var toRemove = storage[id].Where(x => x == null || x.Id == id || string.IsNullOrEmpty(x.Id)).ToArray();
            foreach (var entity in toRemove)
            {
                storage[id].Remove(entity);
                OnRemoved?.Invoke(entity);
            }
            
            if (storage[id].Count == 0)
                storage.Remove(id);
        }

        public void Clear()
        {
            OnAdded = null;
            OnRemoved = null;
            storage.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}