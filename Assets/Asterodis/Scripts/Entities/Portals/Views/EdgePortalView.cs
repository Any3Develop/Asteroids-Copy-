using System;
using Services.PortalService;
using UnityEngine;

namespace Asterodis.Entities.Portals
{
    [RequireComponent(typeof(EdgeCollider2D))]
    public class EdgePortalView : MonoBehaviour, IEdgePortalSceneEntity
    {
        [SerializeField] private Transform selfContainer;
        [SerializeField] private EdgeCollider2D portalCollider;

        public event Action<ITeleportable> OnEnter;
        
        public event Action<ITeleportable> OnExit;

        public Transform Container => selfContainer;
        
        public string Id { get; private set; }


        public void SetOwnerId(string id)
        {
            Id = id;
        }

        public void SetEdge(Edge value)
        {
            portalCollider.points = new Vector2[] {value.PointA, value.PointB};
            portalCollider.edgeRadius = value.EdgeRadius;
        }
        
        private void OnDestroy()
        {
            Id = string.Empty;
            OnEnter = null;
            OnExit = null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null || !other.TryGetComponent(out ITeleportable teleportable))
                return;
            
            OnEnter?.Invoke(teleportable);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == null || !other.TryGetComponent(out ITeleportable teleportable))
                return;
            
            OnExit?.Invoke(teleportable);
        }
    }
}