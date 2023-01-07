using System;

namespace Services.EntityService.Pool
{
    public interface IEntityPoolable : IEntity, IDisposable
    {
        string Name { get; }
        void Spawn();
        void Despawn();
    }
}