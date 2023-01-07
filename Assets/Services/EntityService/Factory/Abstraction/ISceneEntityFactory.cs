using UnityEngine;

namespace Services.EntityService.Factory
{
    public interface ISceneEntityFactory<out T> where T : ISceneEntity
    {
        string ResourcePath { get; }
        TEntity Create<TEntity>(string name = null, Transform parent = null, params object[] args);
        T Create(string name, Transform parent = null, params object[] args);
    }
}