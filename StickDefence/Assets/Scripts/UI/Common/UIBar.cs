using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    public class UIBar : MonoBehaviour
    {
        [SerializeField] private Slider _barSlider;

        public void SetBarFiilAmount(float amount)
        {
            _barSlider.value = amount;
        }


    }
}