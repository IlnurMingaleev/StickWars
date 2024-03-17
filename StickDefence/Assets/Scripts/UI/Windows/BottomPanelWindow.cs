using Models.Merge;
using TMPro;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

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

        [Inject] private IWindowManager _windowManager;

        private WindowPriority Priority = WindowPriority.TopPanel;

        private MergeController _mergeController;
        protected override void OnActivate()
        {
            if(_mergeController == null) _mergeController = FindObjectOfType<MergeController>();
            base.OnActivate();
            InitWindowButtons();
        }

        private void InitWindowButtons()
        {
            _stickmanShopBtn.OnClickAsObservable.Subscribe(_ =>
                {
                    _windowManager.GetWindow<StickmanShopWindow>().Init(_mergeController.GetComponent<IPlaceableUnit>());
                    _windowManager.Show<StickmanShopWindow>();
                })
                .AddTo(ActivateDisposables);
        }

        public void SetBoxImageFill(float value)
        {
            _boxImage.fillAmount = value;
        }
        
    }
}