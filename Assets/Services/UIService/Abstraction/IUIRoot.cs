using UnityEngine;

namespace Services.UIService
{
    public interface IUIRoot
    {
        Camera UICamera { get; }
        Canvas UICanvas { get; }

        RectTransform DeactivatedContainer { get; }
        RectTransform ButtomContainer { get; }
        RectTransform MiddleContainer { get; }
        RectTransform TopContainer { get; }
        RectTransform SafeArea { get; }
    }
}