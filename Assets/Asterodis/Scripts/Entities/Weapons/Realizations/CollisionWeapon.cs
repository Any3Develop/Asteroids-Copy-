using System;
using System.Linq;
using Asterodis.Settings;
using Services.EntityService;
using Services.Extensions;
using Services.SettingsService;
using Zenject;

namespace Asterodis.Entities.Weapons
{
    public class CollisionWeapon : IWeapon, IInitializable
    {
        private readonly ISettingsRepository settingsRepository;
        private IContactableSceneEntity[] sceneEntities;
        private bool Initialized => !string.IsNullOrEmpty(Id) && !sceneEntities.IsNullOrEmpty();
        private CollisionWeaponSetting weaponSetting;
        private GameSettings gameSettings;
        
        public string Id { get; private set; }

        public CollisionWeapon(ISettingsRepository settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }
        
        void IInitializable.Initialize()
        {
            gameSettings = settingsRepository.Get<GameSettings>();
            weaponSetting = settingsRepository.Get<CollisionWeaponSetting>();
        }
        
        public void Initialize(string ownerId, params IWeaponSceneEntity[] weaponeViews)
        {
            Release();
            Id = ownerId;
            sceneEntities = weaponeViews.OfType<IContactableSceneEntity>().ToArray();
            sceneEntities.ForEach(x=> x.OnContact += OnSceneEntityContact);
        }

        private void OnSceneEntityContact(IContactableSceneEntity contact, IContactableSceneEntity sceneEntity)
        {
            if (!Initialized || contact == null)
                return;

            if (contact is IDamageReceiverEntity damageReceiverEntity
                && (damageReceiverEntity.Id != Id || gameSettings.FriendlyFire))
            {
                SendDamage(damageReceiverEntity, sceneEntity);
            }
            
            // important : handle collision after send damage
            if (contact.Id != Id || weaponSetting.FriendlyCollision)
                OnCollision(contact, sceneEntity);
        }

        public void Dispose()
        {
            Release();
            sceneEntities = null;
            weaponSetting = null;
            gameSettings = null;
        }

        public void Release()
        {
            Id = string.Empty;
            sceneEntities.ForEach(x=> x.OnContact -= OnSceneEntityContact);
            sceneEntities = Array.Empty<IContactableSceneEntity>();
        }

        protected virtual void SendDamage(IDamageReceiverEntity receiver, IEntity sender)
        {
            if (!Initialized)
                return;

            receiver.ReceiveDamage(weaponSetting.Damage, sender);
        }
        
        protected virtual void OnCollision(IContactableSceneEntity contact, IContactableSceneEntity sceneEntity)
        {
            // TODO VFX or some else
        }
    }
}