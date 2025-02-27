﻿using Enums;
using TMPro;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    public class UIBar : MonoBehaviour
    {
        [SerializeField] private Slider _barSlider;
        [SerializeField] private TMP_Text _healthLabel;
        public void SetBarFiilAmount(int currentValue, int maxValue, UnitTypeEnum unitType)
        {
            _barSlider.value = (float) currentValue / maxValue;
            _healthLabel.text = $"{(int)unitType}";
        }
        public void SetBarFiilAmount(int currentValue, int maxValue)
        {
            _barSlider.value = (float) currentValue / maxValue;
            _healthLabel.text = $"{SetScoreExt.ConvertIntToStringValue(currentValue,2)}/{SetScoreExt.ConvertIntToStringValue(maxValue,2)}";
        }

        

    }
}