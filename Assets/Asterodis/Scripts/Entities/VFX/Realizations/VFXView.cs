using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Asterodis.Entities.VFX
{
    public abstract class VFXView : MonoBehaviour, IVfxSceneEntity
    {
        [SerializeField] private Transform selfContainer;
        private CancellationTokenSource tokenSource;
        private Vector3 defaultScale;
        protected CancellationToken Token => tokenSource?.Token ?? CancellationToken.None;

        public string Id { get; private set; }

        public Transform Container => selfContainer;

        public string Name => gameObject.name;

        private void Awake()
        {
            defaultScale = Container.localScale;
            OnAwake();
        }

        public void Spawn()
        {
            if (gameObject)
                gameObject.SetActive(true);

            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            OnSpawned();
        }

        public void Despawn()
        {
            if (gameObject)
                gameObject.SetActive(false);

            Container.localScale = defaultScale;
            Container.rotation = Quaternion.identity;
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            SetPosition(Vector2.zero, Space.World);
            SetOwnerId(string.Empty);
            SetParent(null);
            tokenSource = null;
            OnDespawned();
        }

        public void SetOwnerId(string id)
        {
            Id = id;
        }

        public void SetPosition(Vector3 value, Space space)
        {
            switch (space)
            {
                case Space.World:
                    selfContainer.position = value;
                    break;
                case Space.Self:
                    selfContainer.localPosition = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }

        public void SetRotation(Quaternion value, Space space)
        {
            switch (space)
            {
                case Space.World:
                    selfContainer.rotation = value;
                    break;
                case Space.Self:
                    selfContainer.localRotation = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }

        public void SetParent(Transform value, bool stays = false)
        {
            selfContainer.SetParent(value, stays);
        }

        public abstract Task PlayAsync();

        public abstract Task StopAsync();

        protected void OnDestroy()
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = null;
            SetOwnerId(string.Empty);
            OnDisposed();
        }

        protected virtual void OnAwake() {}
        protected virtual void OnDisposed() {}
        protected virtual void OnSpawned() {}
        protected virtual void OnDespawned() {}

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}