using System.Threading.Tasks;
using DG.Tweening;
using Services.Extensions;
using TMPro;
using UnityEngine;

namespace Asterodis.Entities.VFX
{
    public class TextVfxView : VFXView
    {
        [SerializeField] private TextMeshProUGUI textPlaceHolder;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float offset = 50;
        private Color defaultColor;
        private Tween currentTween;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            defaultColor = textPlaceHolder.color;
        }

        public void SetText(string value)
        {
            textPlaceHolder.text = value;
        }

        public override async Task PlayAsync()
        {
            currentTween?.Kill();
            currentTween = DOTween
                .Sequence()
                .Insert(0, Container.DOMoveY(Container.position.y + offset, duration))
                .Insert(0, DoFade())
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
            textPlaceHolder.color = defaultColor;
        }

        private Tween DoFade()
        {
            return DOVirtual.Float(textPlaceHolder.color.a, 0f, duration, Setlpha);

            void Setlpha(float value)
            {
                var color = textPlaceHolder.color;
                color.a = value;
                textPlaceHolder.color = color;
            }
        }
    }
}