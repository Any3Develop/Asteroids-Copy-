using System;
using UnityEngine;

namespace Services.UIService
{
    public abstract class UIBaseAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationHelperPreset preset;
        [SerializeField] protected AnimationSettings settings;

        public abstract void Play(Action onEnd = null);

        public abstract void Backward(Action onEnd = null);

        public abstract void ResetValues();

        [ContextMenu("Reset Settings")]
        protected void ResetSetting()
        {
            settings = new AnimationSettings();
        }

        protected virtual void Awake()
        {
            if (preset)
            {
                if (settings.delay == 0)
                {
                    settings.delay = preset.Settings.delay;
                }

                if (settings.durationIn == 0)
                {
                    settings.durationIn = preset.Settings.durationIn;
                }

                if (settings.durationOut == 0)
                {
                    settings.durationOut = preset.Settings.durationOut;
                }
            }
        }
    }
}