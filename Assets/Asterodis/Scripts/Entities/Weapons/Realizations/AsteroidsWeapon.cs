using System.Linq;
using Asterodis.Entities.Movements;
using Asterodis.GameBuilder;
using Asterodis.Settings;
using Services.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asterodis.Entities.Weapons
{
    public class AsteroidsWeapon : ProjectileWeaponBase
    {
        private int shardsLeft;
        private int sceneEntityTurn;
        private AsteroidWeaponSetting weaponSetting;
        private ITask currentTask;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            sceneEntityTurn = 0;
            weaponSetting = (AsteroidWeaponSetting) WeaponSetting;
            shardsLeft = weaponSetting.ShardsSteps.Range()
                .Aggregate(1, (total, i) => total + weaponSetting.DevideFactor * i) * WeaponSceneEntities.Length;
            currentTask?.Dispose();
            currentTask = AbstractFactory.Create<WhileAllEntityDestoyed>(Id, shardsLeft, typeof(AsteroidProjectileView));
            GameContext.AddTask(currentTask);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            weaponSetting = null;
            shardsLeft = 0;
            sceneEntityTurn = 0;
            currentTask?.Dispose();
            currentTask = null;
        }
        
        protected override void OnReleased()
        {
            base.OnReleased();
            currentTask?.Dispose();
            currentTask = null;
        }

        public override bool CanAttack(params object[] args)
        {
            if (!Initialized)
                return false;

            var shardIndex = args.OfType<int>().FirstOrDefault();
            return shardsLeft > 0 && shardIndex + 1 <= weaponSetting.ShardsSteps;
        }

        protected override IWeaponSceneEntity GetCurrentSceneEntity()
        {
            return WeaponSceneEntities[sceneEntityTurn++ % WeaponSceneEntities.Length];
        }

        protected override IProjectileSceneEntity CreateProjectileSceneEntity(params object[] args)
        {
            if (!Initialized)
                return default;

            var shardIndex = args.OfType<int>().FirstOrDefault();
            var position = shardIndex > 0
                ? args.OfType<Vector3>().FirstOrDefault()
                : GetCurrentSceneEntity().Aim.position;

            var projectileSpeed = WeaponSetting.ProjectileSpeed + shardIndex * weaponSetting.ShardsSpeedMultiplier;
            var projectileView = ProjectilePool.Spawn<AsteroidProjectileView>();
            var projectileTransform = projectileView.Container;
            var movement = AbstractFactory.Create<LinearMovement>(Id, projectileTransform, projectileSpeed);

            projectileTransform.position = position;
            projectileTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-180f, 180f));
            projectileView.SetTag($"Asteroid_{shardIndex}");
            projectileView.SetIndex(shardIndex);
            projectileView.SetMovement(movement);
            projectileView.SetOwnerId(Id);
            return projectileView;
        }
        
        protected override void OnCollision(IContactableSceneEntity contact, IContactableSceneEntity projectile)
        {
            if (!Initialized)
                return;

            if (projectile is not IIndexedEntity indexedEntity)
                return;

            var nextIndex = indexedEntity.Index + 1;
            var position = projectile.Container.position;
            var radius = weaponSetting.ProjectileSize / (nextIndex * weaponSetting.DevideFactor);
            PlayDestoryVfxAsync(projectile);
            ProjectileDespawn(projectile, contact);
            shardsLeft--;
            
            for (var i = 0; i < weaponSetting.DevideFactor; i++)
            {
                var nextPosititon = position;
                var angle = ((float)i / weaponSetting.DevideFactor) * 180f;
                nextPosititon.x = position.x + (radius * Mathf.Cos(angle / (180f / Mathf.PI)));
                nextPosititon.y = position.y + (radius * Mathf.Sin(angle / (180f / Mathf.PI)));
                Fire(nextIndex, nextPosititon);
            }
        }
    }
}