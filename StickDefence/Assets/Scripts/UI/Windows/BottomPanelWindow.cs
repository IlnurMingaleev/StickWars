using System;
using Enums;
using Helpers.Time;
using I2.Loc;
using Models.Battle.Boosters;
using Models.Controllers;
using Models.DataModels;
using Models.DataModels.Data;
using Models.IAP;
using Models.Merge;
using Models.Player;
using Models.Player.PumpingFragments;
using Models.SO.Core;
using TMPro;
using TonkoGames.Controllers.Core;
using TonkoGames.Sound;
using Tools.Extensions;
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
        [SerializeField] private TMP_Text _stickmanPrice;
        [SerializeField] private TMP_Text _levelLabel;

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
        public UIButton FortuneWheelBtn => _fortuneWheelBtn;
        public UIButton MergeBtn => _mergeBtn;
        public Transform WalletOrigin => _moneyText.transform;
        
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private ConfigManager _configManager;
        [Inject] private IPlayer _player;
        [Inject] private IIAPService _iapService;
        [Inject] private ISoundManager _soundManager;
        private WindowPriority Priority = WindowPriority.TopPanel;
        private MergeController _mergeController;
        private StickmanShopWindow _stickmanShopWindow;
        private BoosterManager _boosterManager;
        private SceneInstances _sceneInstances;
        private CompositeDisposable _quicKBuyBtnDisposable = new CompositeDisposable();
        public UIBooster TimerUI => _timerUI;
        private ReactiveDictionary<BoosterTypeEnum, UIBooster> _boostersDictionary =
            new ReactiveDictionary<BoosterTypeEnum, UIBooster>(); 
        public IReadOnlyReactiveDictionary<BoosterTypeEnum, UIBooster> BoosterDictionary => 
            _boostersDictionary;
        public event Action UpgradeWallClickedEvent;

        protected override void OnActivate()
        {
          
            if (!_stickmanShopWindow) _stickmanShopWindow = _manager.GetWindow<StickmanShopWindow>();
            base.OnActivate();
            InitWindowButtons();
            InitWallUpgradeButtonClick(UpgradeWallClickedEvent);
            InitAlertEvents();
            _manager.GetWindow<StickmanShopWindow>().StartAdTimer();
            _stickmanShopWindow.PerksAlert.Subscribe(perkFlag =>
            { 
                AlertShopBtn( (perkFlag|| _stickmanShopWindow.UnitsAlert.Value));
            }).AddTo(ActivateDisposables);
            _stickmanShopWindow.UnitsAlert.Subscribe(unitFlag =>
            { 
                AlertShopBtn( (unitFlag || _stickmanShopWindow.PerksAlert.Value));
            }).AddTo(ActivateDisposables);
            if (_sceneInstances != null)
            {
                if (_mergeController == null) _mergeController = _sceneInstances.MergeController;
                if (_boosterManager == null) _boosterManager = _sceneInstances.BoosterManager;
                _rocketSkillBtn.OnClickAsObservable
                    .Subscribe(_ => _sceneInstances.SkillLifetimeController.StartAiming(SkillTypesEnum.Rocket))
                    .AddTo(ActivateDisposables);
                _greandesSkillBtn.OnClickAsObservable
                    .Subscribe(_ => _sceneInstances.SkillLifetimeController.StartAiming(SkillTypesEnum.Grenade))
                    .AddTo(ActivateDisposables);
                _poisonSkillBtn.OnClickAsObservable
                    .Subscribe(_ => _sceneInstances.SkillLifetimeController.StartAiming(SkillTypesEnum.Gas))
                    .AddTo(ActivateDisposables);
            }
        }

        public void Init(SceneInstances sceneInstances)
        {
            _sceneInstances = sceneInstances;
        }

        private void InitWindowButtons()
        { 
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
            UpdateQuickBuyBtn();
            _dataCentralService.StatsDataModel.CoinsCount.Subscribe(money => UpdateMoneyLabel(money)).AddTo(ActivateDisposables);
            
        }

        public void UpdateQuickBuyBtn()
        {
            PlayerUnitTypeEnum playerUnitType = (_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value >= PlayerUnitTypeEnum.Four)?(PlayerUnitTypeEnum)((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3): PlayerUnitTypeEnum.One;
            _quicKBuyBtnDisposable.Clear();
            _quickBuyBtn.OnClickAsObservable.Subscribe(_ => { OnClickQuickBuyBtn(playerUnitType); }).AddTo(_quicKBuyBtnDisposable);
            UpdateMainBuyButton(playerUnitType);
        }

        private void OnClickQuickBuyBtn(PlayerUnitTypeEnum playerUnitType)
        {
            BuyStickman(_configManager.UnitsStatsSo.DictionaryStickmanConfigs[playerUnitType], playerUnitType);
        }

        private void UpdateMainBuyButton(PlayerUnitTypeEnum playerUnitType)
        {
            _levelLabel.text = $"{(int) playerUnitType }";
            StickmanStatsConfig stickmanStatsConfig =
                _configManager.UnitsStatsSo.StickmanUnitsStatsConfigs[(int) playerUnitType-1];
            PumpingPerkData perkData = _player.Pumping.GamePerks[PerkTypesEnum.DecreasePrice]; 
            _stickmanPrice.text = $"Buy: {(int)(stickmanStatsConfig.Price *(1 - (perkData.Value/100)))}";
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
            _dataCentralService.StatsDataModel.CoinsCount.Subscribe(coins =>
            {
                bool alertFlag = false;
                for(int index = 0; index < 3; index++)
                {
                    PerkTypesEnum perkType = (PerkTypesEnum) index;
                    PlayerPerkConfigModel perkConfigModel = _configManager.PumpingConfigSo.GamePerks[perkType];
                    PerkData perkData = _dataCentralService.PumpingDataModel.PerksReactive[perkType];
                    var nextLevel = perkData.PerkLevel + 1;
                    var perkCost = perkConfigModel.BaseValue + nextLevel * perkConfigModel.AdditionalCost;
                   
                    alertFlag = alertFlag || (coins >= perkCost);
                  
                }
                _stickmanShopWindow.AlertPerksTab(alertFlag);
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
                        _dataCentralService.StatsDataModel.MinusCoinsCount(_player.Pumping.WallData[WallTypeEnum.Basic].Cost);
                        _soundManager.PlayMoneySoundOneShot();
                        action?.Invoke();
                    }
                }


            }).AddTo(ActivateDisposables);
        }

        private void ShowNotEnoughFundsWarning()
        {
           
        }
        
        private void BuyStickman(StickmanStatsConfig stickmanStatsConfig, PlayerUnitTypeEnum playerUnitType)
        {
            if (stickmanStatsConfig.Price <= _dataCentralService.StatsDataModel.CoinsCount.Value)
            {
                if (_mergeController)
                {
                    _mergeController.PlaceDefinedItem((int)playerUnitType);
                    _dataCentralService.StatsDataModel.MinusCoinsCount(stickmanStatsConfig.Price);
                    _soundManager.PlayMoneySoundOneShot();
                }
            }
        }

        #region UpdateUIElements
        private void UpdateMoneyLabel(int money)
        {
            _moneyText.text = $"{SetScoreExt.ConvertIntToStringValue(money,2)}";
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
            _wallUpgradeCost.text = $"{SetScoreExt.ConvertIntToStringValue(cost,2)}";
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
                uiBooster.TimerText.text = String.Format("{0:00}:{1:00}",minutes,sec);
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

        protected override void OnDeactivate()
        {
            _quicKBuyBtnDisposable.Clear();
        }

        public void SetGrenadeBtnInteractability(bool value)
        {
            _greandesSkillBtn.IsInteractable = value;
        }
        public void SetGasBtnInteractability(bool value)
        {
            _poisonSkillBtn.IsInteractable = value;
        }
        public void SetRocketBtnInteractability(bool value)
        {
            _rocketSkillBtn.IsInteractable = value;
        }
    }
}