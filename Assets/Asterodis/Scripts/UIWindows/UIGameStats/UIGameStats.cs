using Services.UIService;
using TMPro;
using UnityEngine;

namespace Asterodis.UIWindows
{
    public class UIGameStats : UIBaseWindow
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI scoreText;

        public void SetLevelText(string value)
        {
            levelText.text = value;
        }

        public void SetScoreText(string value)
        {
            scoreText.text = value;
        }
    }
}