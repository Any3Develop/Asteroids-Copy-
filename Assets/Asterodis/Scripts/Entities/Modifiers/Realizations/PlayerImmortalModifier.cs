using System;
using System.Collections.Generic;
using System.Linq;
using Asterodis.Entities.Players;
using DG.Tweening;
using Services.EntityService.Pool;
using Zenject;

namespace Asterodis.Entities.Modifiers
{
    public class PlayerImmortalModifier : IModifier, IInitializable
    {
        private readonly IEntityPool<IPlayerSceneEntity> entityPool;
        private readonly Dictionary<IEntityPoolable, Tween> activeImmortals;
        private Action onComplete;
        private bool disposed;
        
        public PlayerImmortalModifier(IEntityPool<IPlayerSceneEntity> entityPool)
        {
            this.entityPool = entityPool;
            activeImmortals = new Dictionary<IEntityPoolable, Tween>();
        }
        
        public void Initialize()
        {
            entityPool.OnDespawned += OnEntityDespawned;
        }

        public void Apply(string ownerId)
        {
            var entityViews = entityPool.GetActiveEntities(ownerId);
            if (entityViews.Length == 0)
                return;
            
            foreach (var entityView in entityViews)
            {
                ApplyImmortal(entityView);
            }
        }

        public void OnComplete(Action value)
        {
            onComplete += value;
        }
        
        private void ApplyImmortal(IEntityPoolable entity)
        {
            if (entity is not (IContactableSceneEntity contactable and IGraphicsEntity graphics))
                return;
            
            var fromContactable = contactable.IsContactable;
            contactable.SetActiveContacts(false);
            var fromAlpha = graphics.Alpha;
            var flickerDuration = 0.1f;
            var repeatCount = 25;
            var tween = DOVirtual
                .Float(fromAlpha, 0, flickerDuration, alpha => graphics.SetAlpha(alpha))
                .SetLoops(repeatCount, LoopType.Yoyo)
                .OnKill(Reset)
                .OnComplete(Reset)
                .SetAutoKill(true)
                .Play();
            
            activeImmortals.Add(entity, tween);
            
            void Reset()
            {
                activeImmortals.Remove(entity);
                graphics.SetAlpha(fromAlpha);
                contactable.SetActiveContacts(fromContactable);
                Dispose();
            }
        }

        private void OnEntityDespawned(IEntityPoolable entity)
        {
            if (entity == null || !activeImmortals.ContainsKey(entity)) 
                return;
            
            var tween = activeImmortals[entity];
            tween?.Kill();
        }

        public void Dispose()
        {
            if (disposed)
                return;
            
            disposed = true;
            entityPool.OnDespawned -= OnEntityDespawned;
            foreach (var tween in activeImmortals.Values.ToArray())
            {
                tween?.Kill();
            }
            activeImmortals.Clear();
            onComplete?.Invoke();
            onComplete = null;
        }
    }
}