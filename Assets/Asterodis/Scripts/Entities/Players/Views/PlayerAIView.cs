using UnityEngine;

namespace Asterodis.Entities.Players
{
    public class PlayerAIView : PlayerBaseView, ITaggedEntity
    {
        [SerializeField] private Transform vfxPoint;
        public Transform VfxPoint => vfxPoint;
        public string Tag { get; private set; }

        public void SetTag(string value)
        {
            Tag = value;
        }

        protected override void OnDespawned()
        {
            base.OnDespawned();
            SetTag(string.Empty);
        }
    }
}