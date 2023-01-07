using System;
using DG.Tweening;
using UnityEngine;

namespace Services.UIService.Animations
{
    public class UIFadeInOutAnimation : UIBaseAnimation
    {
        [SerializeField] private CanvasGroup wholeGroup;
        private Tween fadeInOutTween;

        public override void Play(Action onEnd = null)
        {
            fadeInOutTween?.Kill();
            fadeInOutTween = wholeGroup
                .DOFade(1, settings.durationIn)
                .SetEase(settings.EaseIn)
                .SetDelay(Mathf.Abs(settings.delay))
                .SetAutoKill(true)
                .OnComplete(() => onEnd?.Invoke())
                .OnKill(() => onEnd?.Invoke())
                .Play();
        }

        public override void Backward(Action onEnd = null)
        {
            fadeInOutTween?.Kill();
            fadeInOutTween = wholeGroup
                .DOFade(0, settings.durationOut)
                .SetEase(settings.EaseOut)
                .SetDelay(Mathf.Abs(settings.delay))
                .SetAutoKill(true)
                .OnComplete(() => onEnd?.Invoke())
                .OnKill(() => onEnd?.Invoke())
                .Play();
        }

        public override void ResetValues()
        {
            wholeGroup.alpha = 0f;
            fadeInOutTween?.Kill();
        }
    }
}