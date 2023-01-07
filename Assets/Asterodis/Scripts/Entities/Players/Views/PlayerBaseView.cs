using System;
using Asterodis.Entities.Movements;
using Asterodis.Entities.Weapons;
using Services.EntityService;
using Services.PortalService;
using UnityEngine;

namespace Asterodis.Entities.Players
{
    public abstract class PlayerBaseView : MonoBehaviour, IPlayerSceneEntity, IContactableSceneEntity,
        IMovableSceneEntity, IWeaponSceneEntity, IDamageReceiverEntity, ITeleportable
    {
        [SerializeField] private Transform selfContainer;
        [SerializeField] private Transform aimContainer;
        [SerializeField] protected Collider2D contactCollider;
        protected IDamageHandler DamageHandler;
        protected IMovement Movement;

        public Transform Aim => aimContainer;
        
        public Transform Container => selfContainer;
        
        public Vector3 Origin => Container ? Container.position : Vector2.zero;

        public bool IsContactable => contactCollider && contactCollider.enabled;

        public string Name => gameObject ? gameObject.name : string.Empty;
        
        public string Id { get; private set; }
        
        public event Action<IContactableSceneEntity, IContactableSceneEntity> OnContact;

        protected void Awake()
        {
            OnAwake();
        }

        protected void Update()
        {
            Movement?.Update();
        }
        
        public void Teleport(Vector3 point)
        {
            Movement?.SetPosition(point);
        }
        
        public void ReceiveDamage(int value, IEntity killer)
        {
            DamageHandler?.HandleDamage(value, killer);
        }

        public void SetMovement(IMovement value)
        {
            Movement = value;
        }

        public void SetActiveContacts(bool value)
        {
            if (!contactCollider)
                return;

            contactCollider.enabled = value;
        }

        public void SetDamageHandler(IDamageHandler value)
        {
            DamageHandler = value;
        }
        
        public void SetOwnerId(string ownerId)
        {
            Id = ownerId;
        }

        public void Spawn()
        {
            if (Container)
                Container.gameObject.SetActive(true);

            SetActiveContacts(true);
            
            OnSpawned();
        }

        public void Despawn()
        {
            if (Container)
                Container.gameObject.SetActive(false);

            Id = string.Empty;
            DamageHandler = null;
            OnContact = null;
            Movement?.Dispose();
            Movement = null;
            SetActiveContacts(false);
            OnDespawned();
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Id = string.Empty;
            DamageHandler = null;
            OnContact = null;
            Movement?.Dispose();
            Movement = null;
            OnDisposed();
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (other == null || other.gameObject == null || !gameObject.activeSelf)
                return;

            if (other.gameObject.TryGetComponent(out IContactableSceneEntity contact))
                OnContactTrigger(contact);
        }
        
        protected virtual void OnAwake(){}
        protected virtual void OnSpawned(){}
        protected virtual void OnDespawned(){}
        protected virtual void OnDisposed(){}

        protected virtual void OnContactTrigger(IContactableSceneEntity contact)
        {
            OnContact?.Invoke(contact, this);
        }
    }
}