using System.Threading.Tasks;
using DG.Tweening;
using Services.Extensions;
using UnityEngine;

namespace Asterodis.Entities.VFX
{
    public class RocketBoosterVfxView : VFXView
    {
        [SerializeField] private SpriteRenderer graphic;
        [SerializeField] private float duration = 0.7f;
        private Color graphicDefaultColor;
        private Vector3 graphicDefaultScale;
        private Transform graphicTransform;
        private Tween currentTween;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            graphicDefaultColor = graphic.material.color;
            graphicTransform = graphic.transform;
            graphicDefaultScale = graphicTransform.localScale;
        }

        public override async Task PlayAsync()
        {
            currentTween?.Kill();
            currentTween = DOTween
                .Sequence()
                .Insert(0, graphic.material.DOFade(0, duration))
                .Insert(0, graphicTransform.DOScale(0f, duration))
                .SetAutoKill(true)
                .Play();

            await currentTween.Async(Token);
        }

        public override Task StopAsync()
        {
            currentTween?.Kill();
            currentTween = null;
            return Task.CompletedTask;
        }

        protected override void OnDespawned()
        {
            base.OnDespawned();
            graphic.material.color = graphicDefaultColor;
            graphicTransform.localScale = graphicDefaultScale;
            StopAsync();
        }
    }
}