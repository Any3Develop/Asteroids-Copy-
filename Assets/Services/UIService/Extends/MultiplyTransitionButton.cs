using UnityEngine;
using UnityEngine.UI;

namespace Services.UIService.Extends
{
    public class MultiplyTransitionButton : Button
    {
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            var targetColor = state switch
            {
                SelectionState.Disabled => colors.disabledColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Normal => colors.normalColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Selected => colors.selectedColor,
                _ => Color.white
            };

            foreach (var graphic in GetComponentsInChildren<Graphic>())
            {
                graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
            }
        }
    }
}