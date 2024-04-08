using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Common
{
    public class UIBooster : MonoBehaviour
    {
        [SerializeField] private BoosterTypeEnum _boosterTypeEnum;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Image _boosterImage;
        [SerializeField] private TextMeshProUGUI _boostersQuantity;

        public TextMeshProUGUI TimerText => _timerText;
        public Image BoosterImage => _boosterImage;
        public TextMeshProUGUI BoosterQuantity => _boostersQuantity;
        public BoosterTypeEnum BoosterTypeEnum => _boosterTypeEnum;

        public void SetBoosterTypeEnum(BoosterTypeEnum boosterTypeEnum)
        {
            _boosterTypeEnum = boosterTypeEnum;
        }

    }
}