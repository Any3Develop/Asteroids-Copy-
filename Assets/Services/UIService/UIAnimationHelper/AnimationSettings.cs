using System;
using DG.Tweening;

namespace Services.UIService
{
    [Serializable]
    public struct AnimationSettings
    {
        public float durationIn;
        public float durationOut;
        public Ease EaseIn;
        public Ease EaseOut;
        public float delay;
    }
}