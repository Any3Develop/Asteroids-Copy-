using Services.UIService;
using UnityEngine;

namespace Asterodis.UIWindows
{
    public class UIStatistics : UIBaseWindow
    {
        [SerializeField] private Transform contentContainer;
        public Transform ContentContainer => contentContainer;
    }
}