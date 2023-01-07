using UnityEngine;

namespace Services.UIService
{
    public class UIRoot : MonoBehaviour, IUIRoot
    {
        [SerializeField] private Camera uiCamera;
        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private RectTransform deactivatedContainer;
        [SerializeField] private RectTransform bottomContainer;
        [SerializeField] private RectTransform middleContainer;
        [SerializeField] private RectTransform topContainer;
        [SerializeField] private RectTransform safeArea;

        public Camera UICamera => uiCamera;
        public Canvas UICanvas => uiCanvas;

        public RectTransform DeactivatedContainer => deactivatedContainer;
        public RectTransform ButtomContainer => bottomContainer;
        public RectTransform MiddleContainer => middleContainer;
        public RectTransform TopContainer => topContainer;
        public RectTransform SafeArea => safeArea;
    }
}