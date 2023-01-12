using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asterodis.Entities.Movements;
using Asterodis.Entities.Statistics;
using Asterodis.Settings;
using Services.EntityService;
using Services.EntityService.Storage;
using UnityEngine;

namespace Asterodis.Entities.Weapons
{
    public class LaserWeapon : ProjectileWeaponBase
    {
        private readonly IEntityStorage<IStatisticEntity> statisticStorage;
        private readonly List<IStatisticEntity> statisticEntities;
        
        private LaserWeaponSetting weaponSetting;
        private CancellationTokenSource refreshAmmoTask;
        private int currentAmmo;
        private float countDown;
        
        public LaserWeapon(IEntityStorage<IStatisticEntity> statisticStorage)
        {
            this.statisticStorage = statisticStorage;
            statisticEntities = new List<IStatisticEntity>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            weaponSetting = (LaserWeaponSetting) WeaponSetting;
            currentAmmo = weaponSetting.MaxAmmo;
            countDown = weaponSetting.CountDown;

            var laserCountStatistic = AbstractFactory.Create<StatisticEntity>(Id);
            statisticEntities.Add(laserCountStatistic);

            laserCountStatistic.OnRefreshed += () =>
            {
                laserCountStatistic.SetTitle("Laser Weapon");
                laserCountStatistic.SetValue($"[{currentAmmo}]");
            };
            
            statisticStorage.Add(laserCountStatistic);
        }

        protected override void OnDisposed()
        {
            statisticEntities.ForEach(x=>
            {
                statisticStorage.Remove(x);
                x?.Dispose();
            });
            statisticEntities.Clear();
            refreshAmmoTask?.Cancel();
            refreshAmmoTask?.Dispose();
            refreshAmmoTask = null;
            weaponSetting = null;
            countDown = currentAmmo = 0;
        }

        protected override void OnReleased()
        {
            base.OnReleased();
            statisticEntities.ForEach(x=>
            {
                statisticStorage.Remove(x);
                x?.Dispose();
            });
            statisticEntities.Clear();
        }

        private async Task CountDownLaserAmmoAsync(CancellationToken token)
        {
            var laserCountDownStatistic = AbstractFactory.Create<StatisticEntity>(Id);
            laserCountDownStatistic.OnRefreshed += () =>
            {
                laserCountDownStatistic.SetTitle("Laser Time");
                laserCountDownStatistic.SetValue($"[{TimeSpan.FromSeconds(countDown):mm':'ss}]");
            };
            statisticEntities.Add(laserCountDownStatistic);
            statisticStorage.Add(laserCountDownStatistic);
            while (!token.IsCancellationRequested && Application.isPlaying && Initialized)
            {
                while (countDown > 0 && Application.isPlaying && !token.IsCancellationRequested)
                {
                    countDown -= Time.deltaTime;
                    await Task.Yield();
                }

                if (token.IsCancellationRequested) 
                    break;

                currentAmmo++;
                countDown = weaponSetting.CountDown;
                if (currentAmmo < weaponSetting.MaxAmmo) 
                    continue;
                
                break;
            }
            statisticStorage.Remove(laserCountDownStatistic);
            statisticEntities.Remove(laserCountDownStatistic);
            laserCountDownStatistic.Dispose();
        }

        protected override IProjectileSceneEntity CreateProjectileSceneEntity(params object[] args)
        {
            if (!Initialized)
                return default;
            
            var projectileView = ProjectilePool.Spawn<LaserProjectileView>();
            var projectileTransform = projectileView.Container;
            var movement = AbstractFactory.Create<LinearMovement>(Id, projectileTransform, WeaponSetting.ProjectileSpeed);
            var randomAim = GetCurrentSceneEntity().Aim;
            movement.OnMove += passedDistance => OnProjectileMove(passedDistance, projectileView);
            projectileTransform.position = randomAim.position;
            projectileTransform.rotation = randomAim.rotation;
            projectileView.SetMovement(movement);
            projectileView.SetOwnerId(Id);
            projectileView.SetTag(GameContext.PlayerId == Id ? "LaserPlayer" : "LaserEnemy");
            return projectileView;
        }
        
        public override bool CanAttack(params object[] args)
        {
            return Initialized && currentAmmo > 0;
        }

        protected override void OnAttacked()
        {
            if (!Initialized)
                return;
            
            currentAmmo = Mathf.Max(0, currentAmmo -1);
            refreshAmmoTask?.Cancel();
            refreshAmmoTask?.Dispose();
            refreshAmmoTask = new CancellationTokenSource();
            CountDownLaserAmmoAsync(refreshAmmoTask.Token).ConfigureAwait(true);
        }
        
        private void OnProjectileMove(float passedDistance, ISceneEntity projectile)
        {
            if (!Initialized)
                return;

            if (WeaponSetting.Range > 0 && passedDistance >= WeaponSetting.Range)
            {
                PlayDestoryVfxAsync(projectile);
                ProjectileDespawn(projectile, null);
            }
        }
    }
}