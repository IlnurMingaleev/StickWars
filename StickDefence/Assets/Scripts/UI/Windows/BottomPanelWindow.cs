using System;
using Enums;
using I2.Loc;
using Models.DataModels;
using Models.Merge;
using Models.SO.Core;
using TMPro;
using TonkoGames.Controllers.Core;
using UI.Common;
using UI.Content.Shop;
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
        [SerializeField] private UIBar _wallHealthBar;
        [SerializeField] private TMP_Text _wallUpgradeCost;
 
        [Header("Free Box")] 
        [SerializeField] private UIButton _freeBoxBtn;
        [SerializeField] private Image _boxImage;
        
        [Header("Money ")]
        [SerializeField] private TMP_Text _moneyText;
        
        
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private ConfigManager _configManager; 
        private WindowPriority Priority = WindowPriority.TopPanel;
        private MergeController _mergeController;
        private StickmanShopWindow _stickmanShopWindow;
        public event Action UpgradeWallClickedEvent;

        protected override void OnActivate()
        {
            if(_mergeController == null) _mergeController = FindObjectOfType<MergeController>();
            if (!_stickmanShopWindow) _stickmanShopWindow = _manager.GetWindow<StickmanShopWindow>();
            base.OnActivate();
            InitWindowButtons();
            InitWallUpgradeButtonClick(UpgradeWallClickedEvent);
        }

        private void InitWindowButtons()
        {
            _stickmanShopBtn.OnClickAsObservable.Subscribe(_ =>
                {
                   _stickmanShopWindow.Init(_mergeController.GetComponent<IPlaceableUnit>());
                   _manager.Show<StickmanShopWindow>();
                })
                .AddTo(ActivateDisposables);
            PlayerUnitTypeEnum playerUnitType = PlayerUnitTypeEnum.PlayerOne;
            if(_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value >= PlayerUnitTypeEnum.PlayerFour)
                playerUnitType = (PlayerUnitTypeEnum)((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3);
            _quickBuyBtn.OnClickAsObservable.Subscribe(_ =>
                BuyStickman(_configManager.UnitsStatsSo.DictionaryStickmanConfigs[playerUnitType], playerUnitType));
            _dataCentralService.StatsDataModel.CoinsCount.Subscribe(money => UpdateMoneyLabel(money)).AddTo(ActivateDisposables);
        }

        private void UpdateMoneyLabel(int money)
        {
            _moneyText.text = $"{money}";
        }

        public void SetBoxImageFill(float value)
        {
            _boxImage.fillAmount = value;
        }

        public void UpdateWallHealthBar(int currentHealth, int maxHealth)
        {
            _wallHealthBar.SetBarFiilAmount(currentHealth, maxHealth);
        }

        public void UpdateWallCost(int cost)
        {
            _wallUpgradeCost.text = $"{cost}";
        }

        public void InitWallUpgradeButtonClick(Action action)
        {
            _upgradeBarrierLvlBtn.OnClickAsObservable.Subscribe(_ => { action?.Invoke(); }).AddTo(ActivateDisposables);
        }
        private void BuyStickman(StickmanStatsConfig stickmanStatsConfig, PlayerUnitTypeEnum playerUnitType)
        {
            if (stickmanStatsConfig.Price <= _dataCentralService.StatsDataModel.CoinsCount.Value)
            {
                if (_mergeController)
                {
                    _mergeController.PlaceDefinedItem((int)playerUnitType);
                }
            }
            else
            {
                _manager.GetWindow<PopupMessageWindow>().Init(ScriptLocalization.Messages.WarningTitle,ScriptLocalization.Messages.NotEnoughFunds);
                _manager.Show<PopupMessageWindow>();
            }
        }
    }
}