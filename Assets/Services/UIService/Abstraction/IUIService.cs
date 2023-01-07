using UnityEngine;

namespace Services.UIService
{
    public interface IUIService
    {
        /// <summary>
        /// Turns on screen display
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T Show<T>() where T : UIWindow;
        T Show<T>(Transform parent) where T : UIWindow;

        void Move<T>(Transform parent) where T : UIWindow;

        /// <summary>
        /// Turns off screen display
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Hide<T>() where T : UIWindow;

        /// <summary>
        /// Screen creates with specified parent(optional).
        /// </summary>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        void Init<T>(Transform parent = null) where T : UIWindow;
        void InitWindows();
        void LoadWindows();
        
        /// <summary>
        /// Returns screen by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : UIWindow;

        /// <summary>
        /// Removes the screen from the scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Disponse<T>() where T : UIWindow;

    }
}