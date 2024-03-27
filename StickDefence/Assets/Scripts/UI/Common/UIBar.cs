using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    public class UIBar : MonoBehaviour
    {
        [SerializeField] private Slider _barSlider;
        [SerializeField] private TMP_Text _healthLabel;
        public void SetBarFiilAmount(int currentHealth, int maxHealth)
        {
            _barSlider.value = (float) currentHealth / maxHealth;
            _healthLabel.text = $"{currentHealth}/{maxHealth}";
        }
      

    }
}