using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Services.UIService
{
    public class UIService : IUIService
    {
        private readonly IInstantiator instantiator;
        private readonly IUIRoot uiRoot;

        private readonly Dictionary<Type, UIWindow> viewStorage;
        private readonly Dictionary<Type, UIWindow> instViews;

        public UIService(IInstantiator instantiator, IUIRoot uiRoot)
        {
            this.instantiator = instantiator;
            this.uiRoot = uiRoot;
            viewStorage = new Dictionary<Type, UIWindow>();
            instViews = new Dictionary<Type, UIWindow>();
            LoadWindows();
            InitWindows();
        }

        public void LoadWindows()
        {
            var windows = Resources.LoadAll<UIWindow>("UIWindows");
            foreach (var window in windows)
            {
                viewStorage.Add(window.GetType(), window);
            }
        }

        public void InitWindows()
        {
            foreach (var uiWindow in viewStorage)
            {
                Init(uiWindow.Key, uiRoot.DeactivatedContainer);
            }
        }
        
        /// <summary>
        /// Turns on screen display
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Show<T>() where T : UIWindow
        {
            return Show<T>(null);
        }

        /// <summary>
        /// Turns on screen display
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Show<T>(Transform parent) where T : UIWindow
        {
            var type = typeof(T);
            if (instViews.ContainsKey(type))
            {
                var view = instViews[type];
                Move<T>(parent);
                var component = view.GetComponent<T>();

                // always resize to screen size
                var rect = component.transform as RectTransform;
                if (rect != null)
                {
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                }
                
                component.Show();
                return component;
            }
            return null;
        }

        public void Move<T>(Transform parent) where T : UIWindow
        {
            var type = typeof(T);
            if (instViews.ContainsKey(type))
            {
                instViews[type].transform.SetParent(parent ? parent : uiRoot.MiddleContainer , false);
            }
        }

        /// <summary>
        /// Turns off screen display
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Hide<T>() where T : UIWindow
        {
            var type = typeof(T);
            if (instViews.ContainsKey(type))
            {
                var view = instViews[type].GetComponent<T>();
                view.HidedEvent += () =>
                {
                    view.transform.SetParent(uiRoot.DeactivatedContainer);
                };
                view.Hide();
            }
        }

        /// <summary>
        /// Screen creates with specified parent(optional).
        /// </summary>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        public void Init<T>(Transform parent = null) where T : UIWindow
        {
            Init(typeof(T), parent);
        }

        private void Init(Type t, Transform parent = null)
        {
            if (viewStorage.ContainsKey(t) && !instViews.ContainsKey(t))
            {
                parent = parent ? parent : uiRoot.MiddleContainer;
                instViews.Add(t, (UIWindow)instantiator.InstantiatePrefabForComponent(t, viewStorage[t], parent, new object[0]));
            }
        }

        /// <summary>
        /// Returns screen by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : UIWindow
        {
            var type = typeof(T);
            if (instViews.ContainsKey(type))
            {
                var view = instViews[type];
                return view.GetComponent<T>();
            } 
            return null;
        }

        /// <summary>
        /// Removes the screen from the scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Disponse<T>() where T : UIWindow
        {
            var type = typeof(T);
            if (!instViews.ContainsKey(type))
            {
                return;
            }

            var window = instViews[type];
            instViews.Remove(type);
            if (window)
            {
                Object.Destroy(window.gameObject); 
            }
        }
    }
}