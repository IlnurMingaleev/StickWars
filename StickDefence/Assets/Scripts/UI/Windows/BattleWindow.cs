using UI.UIManager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class BattleWindow : Window
    {
        [SerializeField] private Image _boxImage;

        public void SetImageFill(float value)
        {
            _boxImage.fillAmount = value;
        }
    }
}