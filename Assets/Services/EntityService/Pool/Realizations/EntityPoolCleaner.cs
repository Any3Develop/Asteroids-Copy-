using System;
using System.Collections.Generic;

namespace Services.EntityService.Pool
{
    public class EntityPoolCleaner : IEntityPoolCleaner, IDisposable
    {
        private readonly List<IEntityPool> pools;

        public EntityPoolCleaner()
        {
            pools = new List<IEntityPool>();
        }
        public void AddPool(IEntityPool pool)
        {
            pools.Add(pool);
        }

        public void ClearAllPools()
        {
            pools.ForEach(x=> x?.Dispose());
        }

        public void Dispose()
        {
            ClearAllPools();
        }
    }
}