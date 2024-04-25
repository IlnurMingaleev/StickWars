using TMPro;
using Tools.Extensions;
using UnityEngine;

namespace UI.Content.Rewards
{
    public class DynamicSpriteBooster :MonoBehaviour
    {
        [SerializeField] private TMP_Text _valueLabel;
        
        public void UpdateValue(long value)
        {
            _valueLabel.text = SetScoreExt.ConvertIntToStringValue(value, 1);
        }

      
    }
}