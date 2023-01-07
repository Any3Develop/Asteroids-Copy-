using System.Threading.Tasks;
using DG.Tweening;
using Services.Extensions;
using UnityEngine;

namespace Asterodis.Entities.VFX
{
    public class AbstractRibbonsVFXView : VFXView
    {
        [SerializeField] private ParticleSystemRenderer particleRenderer;
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float fadeOutDuration = 1.5f;
        private Material particleMaterialInstance;
        private Material trailMaterialInstance;
        private Tween currentTween;

        protected override void OnAwake()
        {
            base.OnAwake();
            trailMaterialInstance = particleRenderer.trailMaterial;
            particleMaterialInstance = particleRenderer.material;
        }

        public override async Task PlayAsync()
        {
            currentTween?.Kill();
            currentTween = DOTween
                .Sequence()
                .Insert(0, trailMaterialInstance.DOFade(1f, fadeOutDuration))
                .Insert(0, particleMaterialInstance.DOFade(1f, fadeOutDuration))
                .SetAutoKill(true)
                .Play();

            await currentTween.Async(Token);
            currentTween = null;
        }

        public override async Task StopAsync()
        {
            currentTween?.Kill();
            currentTween = DOTween
                .Sequence()
                .Insert(0, trailMaterialInstance.DOFade(0f, fadeInDuration))
                .Insert(0, particleMaterialInstance.DOFade(0f, fadeInDuration))
                .SetAutoKill(true)
                .Play();

            await currentTween.Async(Token);
            currentTween = null;
        }
        
        protected override void OnDisposed()
        {
            base.OnDisposed();
            trailMaterialInstance = null;
            particleMaterialInstance = null;
        }
    }
}