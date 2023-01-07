using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Services.Extensions;
using UnityEngine;

namespace Asterodis.Entities.VFX
{
    public class DestroyVfxView : VFXView
    {
        [SerializeField] private SpriteRenderer[] graphics;
        [SerializeField] private float duration = 1.5f;
        [SerializeField] private float expansion = 1;
        [SerializeField] private Ease expansionEase = Ease.Linear;
        private Transform[] graphicsTransform;
        private Vector3[] graphicsDefaultPositions;
        private Tween currentTween;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            graphicsTransform = graphics.Select(x => x.transform).ToArray();
            graphicsDefaultPositions = graphicsTransform.Select(x => x.localPosition).ToArray();
        }
        
        public override async Task PlayAsync()
        {
            currentTween?.Kill();
            var sequence = DOTween.Sequence();
            foreach (var graphicTransform in graphicsTransform)
            {
                sequence.Insert(0, DoMove(graphicTransform));
            }
            
            foreach (var graphic in graphics)
            {
                sequence.Insert(0, DoFade(graphic));
            }

            currentTween = sequence
                .SetAutoKill(true)
                .Play();
            
            await currentTween.Async();
            currentTween = null;
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
            for (var i = 0; i < graphicsTransform.Length; i++)
            {
                var graphic = graphics[i];
                var graphicTransorm = graphicsTransform[i];
                graphicTransorm.localPosition = graphicsDefaultPositions[i];
                var color = graphic.color;
                color.a = 1;
                graphic.color = color;
            }
        }

        private Tween DoFade(SpriteRenderer graphic)
        {
            return DOVirtual
                .Float(graphic.color.a, 0f, duration, SetAlpha)
                .SetEase(expansionEase);

            void SetAlpha(float value)
            {
                var color = graphic.color;
                color.a = value;
                graphic.color = color;
            }
        }

        private Tween DoMove(Transform graphicTransform)
        {
            var dir = (Container.position - graphicTransform.position).normalized;
            return graphicTransform
                .DOLocalMove(dir * expansion, duration)
                .SetEase(expansionEase)
                .SetAutoKill(true);
        }
    }
}