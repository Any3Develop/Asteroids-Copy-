using UnityEngine;

namespace Asterodis.Entities.Players
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerView : PlayerBaseView, IGraphicsEntity, IAiTargetSceneEntity, ITaggedEntity
    {
        [SerializeField] private Renderer graphicRenderer;
        [SerializeField] private Transform vfxPoint;
        private Material graphicMaterialInstance;
        private float defaultAlpha;
        
        public Transform VfxPoint => vfxPoint;

        public bool IsValidTarget => gameObject && gameObject.activeSelf;

        public float Alpha => graphicMaterialInstance.color.a;
        
        public string Tag { get; private set; }
        
        protected override void OnAwake()
        {
            base.OnAwake();
            graphicMaterialInstance = graphicRenderer.material;
            defaultAlpha = graphicMaterialInstance.color.a;
        }
        
        public void SetTag(string value)
        {
            Tag = value;
        }
        
        public void SetAlpha(float value)
        {
            if (!graphicMaterialInstance)
                return;

            var color = graphicMaterialInstance.color;
            color.a = value;
            graphicMaterialInstance.color = color;
        }
        
        protected override void OnDespawned()
        {
            base.OnDespawned();
            SetAlpha(defaultAlpha);
            SetTag(string.Empty);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            graphicMaterialInstance = null;
        }
    }
}