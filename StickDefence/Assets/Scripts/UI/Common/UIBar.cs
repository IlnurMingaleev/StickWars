using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    public class UIBar : MonoBehaviour
    {
        [SerializeField] private Slider _barSlider;
        [SerializeField] private TMP_Text _healthLabel;
        public void SetBarFiilAmount(int currentValue, int maxValue)
        {
            _barSlider.value = (float) currentValue / maxValue;
            _healthLabel.text = $"{currentValue}/{maxValue}";
        }
      

    }
}