using TMPro;
using Tools.Extensions;
using UnityEngine;

namespace UI.Content.PlayResult
{
    public class ResultRewardBlock : MonoBehaviour
    {
        [SerializeField] private TMP_Text _labelValue;
        [SerializeField] private GameObject _bonusVisual;

        public void SetBonusVisual(bool bonusVisual)
        {
            _bonusVisual.SetActive(bonusVisual);
        }
        public void SetValue(int rewardCount)
        {
            gameObject.SetActive(rewardCount > 0);
            _labelValue.text = SetScoreExt.ConvertIntToStringValue(rewardCount, 1);
        }
    }
}