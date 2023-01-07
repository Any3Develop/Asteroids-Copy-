using System;
using System.Collections.Generic;

namespace Services.EntityService.Storage
{
    public interface IEntityStorage<T> where T : IEntity
    {
        event Action<T> OnAdded;
        event Action<T> OnRemoved;
        T Get(string id);
        IEnumerable<T> GetAll(string id);
        IEnumerable<T> Get();
        IEnumerable<T> GetExcept(params string[] ids);
        bool TryGet(string id, out T result);
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        bool Contains(string id);
        bool Contains(T entity);
        void Remove(T entity);
        void Remove(string id);
        void Clear();
    }
}