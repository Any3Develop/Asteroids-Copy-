using System;
using Services.EntityService;
using Zenject;

namespace Asterodis.GameBuilder
{
    public class WhileAllEntityDestoyed : ITask, IInitializable
    {
        private readonly string ownerId;
        private readonly Type entityType;
        private readonly IGameContext gameContext;
        private readonly int entityMax;
        private int entityLeft;
        public bool IsEnded => entityLeft <= 0;
        public bool Disposed { get; private set; }
        public float Progress => IsEnded ? 1f : (float)(entityMax - entityLeft) / entityMax;

        public WhileAllEntityDestoyed(string ownerId, int entityLeft, Type entityType, IGameContext gameContext)
        {
            entityMax = entityLeft;
            this.entityLeft = entityLeft;
            this.ownerId = ownerId;
            this.entityType = entityType;
            this.gameContext = gameContext;
        }

        public void Initialize()
        {
            gameContext.OnDestoryed += OnEntityDestoryed;
        }
        
        public void Dispose()
        {
            if (Disposed)
                return;
            
            Disposed = true;
            gameContext.OnDestoryed -= OnEntityDestoryed;
            entityLeft = 0;
        }
        
        private void OnEntityDestoryed(IEntity target, IEntity killer)
        {
            if (target?.Id != ownerId || target?.GetType() != entityType)
                return;
            
            entityLeft--;
            if (IsEnded)
                Dispose();
        }
    }
}