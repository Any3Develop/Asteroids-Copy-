using System;
using DG.Tweening;
using UnityEngine;

namespace Services.UIService.Animations
{
    public class UIFlickerAnimation : UIBaseAnimation
    {
        [SerializeField] private int flickCount = 3;
        [SerializeField] private CanvasGroup wholeGroup;
        [SerializeField] private LoopType loopType = LoopType.Yoyo;
        [SerializeField] private bool loop;
        private Tween flickerTween;

        public override void Play(Action onEnd = null)
        {
            var fromAlpha = wholeGroup.alpha;
            wholeGroup.alpha = 1;
            flickerTween?.Kill();
            flickerTween = wholeGroup
                .DOFade(0, settings.durationIn)
                .SetEase(settings.EaseIn)
                .SetDelay(Mathf.Abs(settings.delay))
                .SetLoops(loop ? -1 : flickCount * 2, loopType)
                .SetAutoKill(true)
                .OnComplete(Reset)
                .OnKill(Reset)
                .Play();

            void Reset()
            {
                wholeGroup.alpha = fromAlpha;
                onEnd?.Invoke();
            }
        }

        public override void Backward(Action onEnd = null)
        {
            flickerTween?.Kill();
            onEnd?.Invoke();
        }

        public override void ResetValues()
        {
            flickerTween?.Kill();
        }
    }
}