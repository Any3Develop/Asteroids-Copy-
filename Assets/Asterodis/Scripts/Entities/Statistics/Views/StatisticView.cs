using Services.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asterodis.Entities.Statistics
{
    public class StatisticView : MonoBehaviour, IStatisticSceneEntity
    {
        [SerializeField] private Transform selfContainer;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image imageIcon;
        public string Id { get; private set; }

        public Transform Container => selfContainer;

        public int Index { get; private set; }

        public void SetIndex(int value)
        {
            Index = value;
            
            selfContainer.SetSiblingIndex(value == 0 ? int.MaxValue : value);
        }

        public void SetId(string value)
        { 
            Id = value;
        }

        public void SetTitle(string value)
        {
            titleText.text = value;
        }

        public void SetText(string value)
        {
            valueText.text = value;
        }

        public void SetIcon(Sprite value)
        {
            imageIcon.sprite = value;
        }

        public void SetActiveIcon(bool value)
        {
            imageIcon.gameObject.SetActive(value);
        }

        private void OnDestroy()
        {
            if(imageIcon)
                imageIcon.sprite.DestroySafe();
        }
    }
}