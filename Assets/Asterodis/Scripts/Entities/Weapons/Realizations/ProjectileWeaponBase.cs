using System;
using System.Collections.Generic;
using System.Linq;
using Asterodis.Entities.VFX;
using Asterodis.GameBuilder;
using Asterodis.Settings;
using Services.AbstractFactoryService;
using Services.EntityService;
using Services.EntityService.Pool;
using Services.Extensions;
using Services.SettingsService;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Asterodis.Entities.Weapons
{
    public abstract class ProjectileWeaponBase : IProjectileWeapon
    {
        protected IGameContext GameContext;
        protected IAbstractFactory AbstractFactory;
        protected IWeaponSceneEntity[] WeaponSceneEntities;
        protected IEntityPool<IProjectileSceneEntity> ProjectilePool;
        protected IEntityPool<IVfxSceneEntity> VfxScenEntityPool;
        protected IList<IEntityPoolable> Projectiles;
        protected WeaponSetting WeaponSetting;
        protected GameSettings GameSettings;
        protected float LastAttack;
        protected bool Initialized => !string.IsNullOrEmpty(Id) && WeaponSceneEntities.Any();
        public string Id { get; private set; }


        [Inject] // I do not want to use a constructor, so as not to produce constructor inheritance.
        private void Construct(
            IGameContext gameContext,
            ISettingsRepository settingsRepository,
            IEntityPool<IProjectileSceneEntity> projectilePool,
            IEntityPool<IVfxSceneEntity> vfxScenEntityPool,
            IAbstractFactory abstractFactory)
        {
            GameContext = gameContext;
            ProjectilePool = projectilePool;
            VfxScenEntityPool = vfxScenEntityPool;
            AbstractFactory = abstractFactory;
            WeaponSetting = settingsRepository.Get<WeaponSetting>(GetType().Name);
            GameSettings = settingsRepository.Get<GameSettings>();
            Projectiles = new List<IEntityPoolable>();
            WeaponSceneEntities = Array.Empty<IWeaponSceneEntity>();
        }

        public void Initialize(string ownerId, params IWeaponSceneEntity[] weaponeViews)
        {
            Id = ownerId;
            WeaponSceneEntities = weaponeViews;
            DespawnAll();
            OnInitialized();
        }

        public void Dispose()
        {
            if (!Initialized)
                return;

            Release();
            Id = string.Empty;
            AbstractFactory = null;
            ProjectilePool = null;
            Projectiles = null;
            WeaponSetting = null;
            GameSettings = null;
            LastAttack = 0;
            OnDisposed();
        }

        public void Fire(params object[] args)
        {
            if (!Initialized)
                return;

            if (LastAttack > Time.time)
                return;

            if (!CanAttack(args))
                return;

            LastAttack = Time.time + WeaponSetting.AttackSpeed;
            var projectile = CreateProjectileSceneEntity(args);
            projectile.OnContact += ProjectileContact;
            Projectiles.Add(projectile);
            GameContext.Spawned(projectile);
            OnAttacked();
        }

        public void Release()
        {
            DespawnAll();
            Projectiles.Clear();
            WeaponSceneEntities = Array.Empty<IWeaponSceneEntity>();
            OnReleased();
        }

        public abstract bool CanAttack(params object[] args);

        protected void DespawnAll()
        {
            if (!Initialized)
                return;

            foreach (var projectile in Projectiles.ToArray())
            {
                ProjectileDespawn(projectile, null);
            }
        }

        protected abstract IProjectileSceneEntity CreateProjectileSceneEntity(params object[] args);

        protected void ProjectileDespawn(IEntity projectile, IEntity killer)
        {
            if (!Initialized || projectile is not IEntityPoolable entityPoolable)
                return;
            
            Projectiles.Remove(entityPoolable);
            GameContext.Destoyed(projectile, killer);
            ProjectilePool.DespawnLate(entityPoolable, null);
        }

        protected void ProjectileContact(IContactableSceneEntity contact, IContactableSceneEntity projectile)
        {
            if (!Initialized || contact == null)
                return;
            
            if (contact is IDamageReceiverEntity damageReceiverEntity
                && (damageReceiverEntity.Id != Id || GameSettings.FriendlyFire))
            {
                SendDamage(damageReceiverEntity, projectile);
            }
            
            // important : handle collision after send damage
            if (contact.Id != Id || WeaponSetting.FriendlyCollision)
                OnCollision(contact, projectile);
        }

        protected virtual void SendDamage(IDamageReceiverEntity receiver, IEntity sender)
        {
            if (!Initialized)
                return;

            receiver.ReceiveDamage(WeaponSetting.Damage, sender);
        }

        protected virtual IWeaponSceneEntity GetCurrentSceneEntity()
        {
            if (!Initialized || WeaponSceneEntities.IsNullOrEmpty())
                return default;

            return WeaponSceneEntities[Random.Range(0, WeaponSceneEntities.Length)];
        }

        protected virtual void OnInitialized() {}

        protected virtual void OnDisposed() {}

        protected virtual void OnAttacked() {}

        protected virtual void OnReleased() {}
        
        protected void PlayDestoryVfxAsync(ISceneEntity sceneEntity)
        {
            // the pool does not know that your prefabs are different
            // but the type is the same, so you must specify the name of the prefab.
            var vfxView = VfxScenEntityPool.Spawn<DestroyVfxView>(nameof(DestroyVfxView)); 
            vfxView.SetPosition(sceneEntity.Container.position, Space.World);
            vfxView.SetRotation(sceneEntity.Container.rotation, Space.World);
            HandleVfxAsync();
            
            async void HandleVfxAsync()
            {
                await vfxView.PlayAsync();
                await vfxView.StopAsync();
                VfxScenEntityPool.Despawn(vfxView);
            }
        }

        protected virtual void OnCollision(IContactableSceneEntity contact, IContactableSceneEntity projectile){}
    }
}