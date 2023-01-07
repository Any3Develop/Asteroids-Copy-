using System;
using UnityEngine;

namespace Services.UIService
{
    public abstract class UIBaseWindow : UIWindow
    {
        [SerializeField] protected UIBaseAnimation uiAnimation;
        public event EventHandler CloseEvent;
        
        protected virtual void Awake()
        {
            gameObject.SetActive(false);

            if (uiAnimation != null)
            {
                uiAnimation.ResetValues();
            }
        }

        public override void Show()
        {
            if (uiAnimation != null)
                uiAnimation.ResetValues();
            
            gameObject.SetActive(true);

            if (uiAnimation != null)
                uiAnimation.Play();
        }

        public override void Hide()
        {
            if (uiAnimation != null)
            {
                uiAnimation.Backward(OnHided);
            }
            else
            {
                OnHided();
            }
        }

        public virtual void Close()
        {
            CloseEvent?.Invoke(this, EventArgs.Empty);
            CloseEvent = null;
        }

        protected override void OnHided()
        {
            gameObject.SetActive(false);
            base.OnHided();
        }
    }
}