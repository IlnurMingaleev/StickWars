using TMPro;
using UI.Common;
using UI.UIManager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class BottomPanelWindow: Window
    {
        [Header("Skills")] 
        [SerializeField] private UIButton _rocketSkillBtn;
        [SerializeField] private UIButton _greandesSkillBtn;
        [SerializeField] private UIButton _poisonSkillBtn;

        [Header("Quick Buy Btn")]
        [SerializeField] private UIButton _quickBuyBtn;

        [Header("Stickman Shop Btn")] 
        [SerializeField] private UIButton _stickmanShopBtn;

        [Header("Upgrade BarrierHealth Btn")] 
        [SerializeField] private UIButton _upgradeBarrierLvlBtn;

        [Header("Free Box")] 
        [SerializeField] private UIButton _freeBoxBtn;
        [SerializeField] private Image _boxImage;
        
        [Header("Money ")]
        [SerializeField] private TMP_Text _moneyText;
        
        public void SetBoxImageFill(float value)
        {
            _boxImage.fillAmount = value;
        }
    }
}