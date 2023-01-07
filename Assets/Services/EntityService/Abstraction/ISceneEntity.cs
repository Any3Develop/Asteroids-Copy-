using UnityEngine;

namespace Services.EntityService
{
    public interface ISceneEntity : IEntity
    {
        Transform Container { get; }
    }
}