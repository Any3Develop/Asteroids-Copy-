using System;
using Asterodis.Entities.Movements;
using Services.PortalService;
using UnityEngine;

namespace Asterodis.Entities.Weapons
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ProjectileBaseView : MonoBehaviour, IProjectileSceneEntity, ITeleportable
    {
        [SerializeField] protected Transform selfContainer;
        [SerializeField] protected Collider2D contactCollider;
        private IMovement movement;

        public string Id { get; private set; }

        public string Name => gameObject.name;

        public Transform Container => selfContainer;
        
        public Vector3 Origin => selfContainer.position;
        
        public bool IsContactable => contactCollider && contactCollider.enabled;
        

        public event Action<IContactableSceneEntity, IContactableSceneEntity> OnContact;


        public void Teleport(Vector3 point)
        {
            movement?.SetPosition(point);
        }

        public void SetActiveContacts(bool value)
        {
            if (contactCollider)
                contactCollider.enabled = value;
        }

        public void SetOwnerId(string id)
        {
            Id = id;
        }

        public void SetMovement(IMovement value)
        {
            movement = value;
        }

        public void Spawn()
        {
            if (Container)
                Container.gameObject.SetActive(true);

            OnSpawned();
            SetActiveContacts(true);
        }

        public void Despawn()
        {
            SetActiveContacts(false);

            if (Container)
                Container.gameObject.SetActive(false);

            movement?.Dispose();
            movement = null;
            OnContact = null;
            Id = string.Empty;
            OnDespawned();
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
        
        private void Update()
        {
            movement?.Update();
        }

        private void OnDestroy()
        {
            Despawn();
            OnDisposed();
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (other == null || other.gameObject == null || !gameObject.activeSelf)
                return;

            if (other.gameObject.TryGetComponent(out IContactableSceneEntity entity))
                OnContact?.Invoke(entity, this);
        }

        protected virtual void OnDisposed() {}
        protected virtual void OnSpawned() {}
        protected virtual void OnDespawned() {}
    }
}