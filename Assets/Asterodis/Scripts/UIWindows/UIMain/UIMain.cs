using System;
using Asterodis.Entities.VFX;
using Services.UIService;
using Services.VFXService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asterodis.UIWindows
{
    public class UIMain : UIBaseWindow
    {
        [SerializeField] private Button invisibleButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private UIBaseAnimation messageTextAnimation;
        [SerializeField] private VFXView backgroundVfx; // TODO remove from here
        public event Action OnClick;

        public override void Show()
        {
            base.Show();
            invisibleButton.onClick.AddListener(() => OnClick?.Invoke());
            messageTextAnimation.ResetValues();
            messageTextAnimation.Play();
        }

        public override void Hide()
        {
            base.Hide();
            backgroundVfx.StopAsync();
            invisibleButton.onClick.RemoveAllListeners();
            messageTextAnimation.Backward();
            OnClick = null;
        }

        public void SetTitleText(string value)
        {
            titleText.text = value;
        }

        public void SetMessageText(string value)
        {
            messageText.text = value;
        }

        public void PlayVfx()
        {
            backgroundVfx.PlayAsync();
        }
    }
}