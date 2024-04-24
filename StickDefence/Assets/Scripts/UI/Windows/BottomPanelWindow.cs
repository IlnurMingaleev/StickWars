using System;
using Enums;
using Helpers.Time;
using I2.Loc;
using Models.Battle.Boosters;
using Models.Controllers;
using Models.DataModels;
using Models.IAP;
using Models.Merge;
using Models.Player;
using Models.SO.Core;
using TMPro;
using TonkoGames.Controllers.Core;
using Ui.Common;
using UI.Common;
using UI.Content.Spin;
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
        [SerializeField] private Image _rocketImage;
        [SerializeField] private Image _greandesImage;
        [SerializeField] private Image _poisonSkillImage;
        
        [Header("Quick Buy Btn")]
        [SerializeField] private UIButton _quickBuyBtn;

        [Header("Stickman Shop Btn")] 
        [SerializeField] private UIButton _stickmanShopBtn;
        [SerializeField] private GameObject _alertShopIcon;

        [Header("Upgrade BarrierHealth Btn")] 
        [SerializeField] private UIButton _upgradeBarrierLvlBtn;
        [SerializeField] private UIBar _wallHealthBar;
        [SerializeField] private TMP_Text _wallUpgradeCost;
        [SerializeField] private GameObject _alertWallIcon;
 
        [Header("Free Box")] 
        [SerializeField] private UIButton _freeBoxBtn;
        [SerializeField] private Image _boxImage;
        
        [Header("Money ")]
        [SerializeField] private TMP_Text _moneyText;
        
        [Header("UIBooster")]
        [SerializeField] private UIBooster _timerUI;
        [SerializeField] private Transform _timerParent;

        [Header("Buttons")] 
        [SerializeField] private UIButton _fortuneWheelBtn;
        [SerializeField] private UIButton _mergeBtn;
        
        
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private ConfigManager _configManager;
        [Inject] private IPlayer _player;
        [Inject] private IIAPService _iapService;
        private WindowPriority Priority = WindowPriority.TopPanel;
        private MergeController _mergeController;
        private StickmanShopWindow _stickmanShopWindow;
        private BoosterManager _boosterManager;
        public UIBooster TimerUI => _timerUI;
        private ReactiveDictionary<BoosterTypeEnum, UIBooster> _boostersDictionary =
            new ReactiveDictionary<BoosterTypeEnum, UIBooster>(); 
        public IReadOnlyReactiveDictionary<BoosterTypeEnum, UIBooster> BoosterDictionary => 
            _boostersDictionary;
        public event Action UpgradeWallClickedEvent;

        protected override void OnActivate()
        {
            if (_mergeController == null) _mergeController = SceneInstances.Instance.MergeController;
            if (_boosterManager == null) _boosterManager = SceneInstances.Instance.BoosterManager;
            if (!_stickmanShopWindow) _stickmanShopWindow = _manager.GetWindow<StickmanShopWindow>();
            base.OnActivate();
            InitWindowButtons();
            InitWallUpgradeButtonClick(UpgradeWallClickedEvent);
            InitAlertEvents();
        }
        
        private void InitWindowButtons()
        {
            _rocketSkillBtn.OnClickAsObservable
                .Subscribe(_ => SceneInstances.Instance.AimController.StartAiming(SkillTypesEnum.Rocket))
                .AddTo(ActivateDisposables);
            _greandesSkillBtn.OnClickAsObservable
                .Subscribe(_ => SceneInstances.Instance.AimController.StartAiming(SkillTypesEnum.Grenade))
                .AddTo(ActivateDisposables);
            _poisonSkillBtn.OnClickAsObservable
                .Subscribe(_ => SceneInstances.Instance.AimController.StartAiming(SkillTypesEnum.Gas))
                .AddTo(ActivateDisposables);
            _stickmanShopBtn.OnClickAsObservable.Subscribe(_ =>
                {
                   _stickmanShopWindow.Init(_mergeController.GetComponent<IPlaceableUnit>());
                   _manager.Show<StickmanShopWindow>();
                })
                .AddTo(ActivateDisposables);
            _fortuneWheelBtn.OnClickAsObservable.Subscribe(_ =>
            {
                _manager.Show<LuckySpinWindow>(WindowPriority.AboveTopPanel);
            }).AddTo(ActivateDisposables);
            _mergeBtn.OnClickAsObservable.Subscribe(_ =>
            {
               RewardMergeButton();
            }).AddTo(ActivateDisposables);
            PlayerUnitTypeEnum playerUnitType = PlayerUnitTypeEnum.One;
            if(_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value >= PlayerUnitTypeEnum.Four)
                playerUnitType = (PlayerUnitTypeEnum)((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3);
            _quickBuyBtn.OnClickAsObservable.Subscribe(_ =>
                BuyStickman(_configManager.UnitsStatsSo.DictionaryStickmanConfigs[playerUnitType], playerUnitType));
            _dataCentralService.StatsDataModel.CoinsCount.Subscribe(money => UpdateMoneyLabel(money)).AddTo(ActivateDisposables);
            
        }

        private void InitAlertEvents()
        {
            _dataCentralService.StatsDataModel.CoinsCount.Subscribe(coins =>
            {
                if (coins >= _player.Pumping.WallData[WallTypeEnum.Basic].Cost)
                {
                    _alertWallIcon.SetActive(true);
                }
                else
                {
                    _alertWallIcon.SetActive(false);
                }
            }).AddTo(ActivateDisposables);
            
        }

       

        public void InitWallUpgradeButtonClick(Action action)
        {
            _upgradeBarrierLvlBtn.OnClickAsObservable.Subscribe(_ =>
            {
                if (_player.Pumping.WallData[WallTypeEnum.Basic].Cost <= _dataCentralService.StatsDataModel.CoinsCount.Value)
                {
                    if (!_player.Pumping.WallData[WallTypeEnum.Basic].IsMaxLevel)
                    {
                        _dataCentralService.StatsDataModel.MinusCoinsCount(_player.Pumping.WallData[WallTypeEnum.Basic]
                            .Cost);
                        action?.Invoke();
                    }
                    else
                    {
                        ShowMaxLevelReachedWarning();
                    }
                }
                else
                {
                    ShowNotEnoughFundsWarning();
                }


            }).AddTo(ActivateDisposables);
        }

        private void ShowNotEnoughFundsWarning()
        {
            _manager.GetWindow<PopupMessageWindow>().Init(ScriptLocalization.Messages.WarningTitle,
                ScriptLocalization.Messages.NotEnoughFunds);
            _manager.Show<PopupMessageWindow>();
        }
        
        private void ShowMaxLevelReachedWarning()
        {
            _manager.GetWindow<PopupMessageWindow>().Init(ScriptLocalization.Messages.WarningTitle,
                ScriptLocalization.Messages.MaxLevel);
            _manager.Show<PopupMessageWindow>();
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
               ShowNotEnoughFundsWarning();
            }
        }

        #region UpdateUIElements
        private void UpdateMoneyLabel(int money)
        {
            _moneyText.text = $"{money}";
        }

        public void SetBoxImageFill(float value)
        {
            _boxImage.fillAmount = value;
        }

        public void UpdateWallHealthBar(int currentHealth, int maxHealth, UnitTypeEnum unitType)
        {
            _wallHealthBar.SetBarFiilAmount(currentHealth, maxHealth, unitType);
        }
        public void UpdateWallHealthBar(int currentHealth, int maxHealth)
        {
            _wallHealthBar.SetBarFiilAmount(currentHealth, maxHealth);
        }

        public void UpdateWallCost(int cost)
        {
            _wallUpgradeCost.text = $"{cost}";
        }
        public void UpdateRocketFill(float value)
        {
            _rocketImage.fillAmount = value;
        }

        public void UpdateGrenadeFill(float value)
        {
            _greandesImage.fillAmount = value;
        }

        public void UpdateGasFill(float value)
        {
            _poisonSkillImage.fillAmount = value;
        }

        #endregion

        public void SetTimer(float seconds, BoosterTypeEnum boosterTypeEnum)
        {
            int minutes = TimeHelpers.TimeStampToDataTime((long)seconds).Minute;
            int sec = TimeHelpers.TimeStampToDataTime((long) seconds).Second;
            UIBooster uiBooster = null;
            if (_boostersDictionary.ContainsKey(boosterTypeEnum))
            {
                uiBooster = _boostersDictionary[boosterTypeEnum];
            }
            else
            {
                uiBooster = Instantiate(_timerUI.gameObject, _timerParent).GetComponent<UIBooster>();
                uiBooster.BoosterImage.sprite = _configManager.BoostSpritesSO.BoosterSprites[boosterTypeEnum];
                uiBooster.SetBoosterTypeEnum(boosterTypeEnum);
                _boostersDictionary.Add(boosterTypeEnum,uiBooster);
            }

            if (uiBooster != null)
            {
                uiBooster.gameObject.SetActive(true);
                uiBooster.TimerText.text = String.Format($"{minutes}:{sec}");
            }
        } 
        public void RemoveBooster(BoosterTypeEnum boosterType)
        {
            if(_boostersDictionary.ContainsKey(boosterType))
                _boostersDictionary.Remove(boosterType);
        }

        public void AlertShopBtn(bool value)
        {
            _alertShopIcon.SetActive(value);
        }

        #region Ads

        private void RewardMergeButton()
        {
           
            _iapService.ShowRewardedBreak(RewardSpinBreakComplete);
            _mergeBtn.IsInteractable = false;
        }
        
        private void RewardSpinBreakComplete(bool value)
        {
            if (value)
            {
                _boosterManager.ApplyBooster(BoosterTypeEnum.AutoMerge, () => _mergeBtn.IsInteractable = true);
            }

        }

        #endregion
    }
}