using Asterodis.Entities.Movements;
using Asterodis.Entities.Statistics;
using Services.EntityService;
using Services.EntityService.Storage;

namespace Asterodis.Entities.Weapons
{
    public class GunWeapon : ProjectileWeaponBase
    {
        private readonly IEntityStorage<IStatisticEntity> statisticStorage;
        private IStatisticEntity weaponStatistic;

        public GunWeapon(IEntityStorage<IStatisticEntity> statisticStorage)
        {
            this.statisticStorage = statisticStorage;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var gunStatistic = AbstractFactory.Create<StatisticEntity>(Id);
            gunStatistic.OnRefreshed += () =>
            {
                gunStatistic.SetTitle("Gun Weapon");
                gunStatistic.SetValue("<size=150%>∞");
            };

            weaponStatistic = gunStatistic;
            statisticStorage.Add(gunStatistic);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            if (weaponStatistic != null)
            {
                statisticStorage.Remove(weaponStatistic);
                weaponStatistic?.Dispose();
                weaponStatistic = null;
            }
        }

        protected override IProjectileSceneEntity CreateProjectileSceneEntity(params object[] args)
        {
            if (!Initialized)
                return default;

            var projectileView = ProjectilePool.Spawn<GunProjectileView>();
            var projectileTransform = projectileView.Container;
            var movement =
                AbstractFactory.Create<LinearMovement>(Id, projectileTransform, WeaponSetting.ProjectileSpeed);
            var randomAim = GetCurrentSceneEntity().Aim;
            movement.OnMove += passedDistance => OnProjectileMove(passedDistance, projectileView);
            projectileTransform.position = randomAim.position;
            projectileTransform.rotation = randomAim.rotation;
            projectileView.SetMovement(movement);
            projectileView.SetOwnerId(Id);
            projectileView.SetTag(GameContext.PlayerId == Id ? "GunPlayer" : "GunEnemy");
            return projectileView;
        }

        public override bool CanAttack(params object[] args)
        {
            return Initialized;
        }

        protected override void OnCollision(IContactableSceneEntity contact, IContactableSceneEntity projectile)
        {
            if (!Initialized)
                return;

            PlayDestoryVfxAsync(projectile);
            ProjectileDespawn(projectile, contact);
        }

        private void OnProjectileMove(float passedDistance, IEntity projectile)
        {
            if (!Initialized)
                return;

            if (WeaponSetting.Range > 0 && passedDistance >= WeaponSetting.Range)
                ProjectileDespawn(projectile, null);
        }
    }
}