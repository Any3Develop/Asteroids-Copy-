using System;
using UnityEngine;

namespace Services.UIService
{
    public abstract class UIWindow : MonoBehaviour, IWindow
    {
        public event Action HidedEvent;
        
        public abstract void Show();

        public abstract void Hide();

        protected virtual void OnHided()
        {
            HidedEvent?.Invoke();
            HidedEvent = null;
        }
    }
}