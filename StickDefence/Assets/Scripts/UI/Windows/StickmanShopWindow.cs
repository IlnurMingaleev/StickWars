using System.Collections.Generic;
using Enums;
using I2.Loc;
using Models.DataModels;
using Models.DataModels.Data;
using Models.IAP;
using Models.Merge;
using Models.Player;
using Models.Player.PumpingFragments;
using Models.SO.Core;
using Models.Timers;
using TMPro;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using UI.Common;
using UI.Content.Shop;
using UI.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

namespace UI.Windows
{
    public class StickmanShopWindow : Window
    {
        [Header("StickMan UI Item")] [SerializeField]
        private StickManUIItem _stickManUIItem;

        [Header("PerkUIItem")] [SerializeField]
        private PerkUIItem _perkUIItem;

        [Header("MoneyLabel")]
        [SerializeField] private TMP_Text _moneyLabel;
        
        [Header("Scroll Content Transform")] [SerializeField]
        private Transform _scrollContentTransform;

        [Header("Close window Btn")] [SerializeField]
        private UIButton _closeWindowBtn;

        [Header("Tab Buttons")]
        [SerializeField] private UIButton _stickmanShopBtn;
        [SerializeField] private UIButton _perksShopBtn;
        [SerializeField] private GameObject _stickmanShopBtnAlert;
        [SerializeField] private GameObject _perksShopBtnAlert;
        [Inject] private ConfigManager _configManager;
        [Inject] private IPlayer _player;
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private ICoreStateMachine _coreStateMachine;
        [Inject] private IIAPService _iapService;
        [Inject] private ITimerService _timerService;

        private WindowPriority Priority = WindowPriority.AboveTopPanel;
        private IPlaceableUnit _mergeController;
        private CompositeDisposable _shopDisposable = new CompositeDisposable();
        private CompositeDisposable _buyButtonDisposable = new CompositeDisposable();

        private Dictionary<PlayerUnitTypeEnum, StickManUIItem> _stickManUIItems =
            new Dictionary<PlayerUnitTypeEnum, StickManUIItem>();

        private ITimerModel _timerModel;
        private const float _adTimerCooldown = 10;
        private bool _showAd = false;

        private ReactiveProperty<bool> _perksAlert = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> _unitsAlert = new ReactiveProperty<bool>(false);

        public IReadOnlyReactiveProperty<bool> PerksAlert => _perksAlert;
        public IReadOnlyReactiveProperty<bool> UnitsAlert => _unitsAlert;

        public void Init(IPlaceableUnit mergeController)
        {
            _mergeController = mergeController;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Pause);
            InitWindowButtons();
            InitTextFields();
            InitStickmanTab();
            StartAdTimer();
        }

        private void InitTextFields()
        {
            _dataCentralService.StatsDataModel.CoinsCount.Subscribe(amount =>
            {
                ShowMoneyAmount(amount);
            }).AddTo(ActivateDisposables);
        }

        private void ShowMoneyAmount(int amount)
        {
            _moneyLabel.text = $"{amount}";
        }

        private void InitWindowButtons()
        {
            _closeWindowBtn.OnClickAsObservable.Subscribe(_ => { _manager.Hide(this); }).AddTo(ActivateDisposables);
            _stickmanShopBtn.OnClickAsObservable.Subscribe(_ => { InitStickmanTab(); }).AddTo(ActivateDisposables);
            _perksShopBtn.OnClickAsObservable.Subscribe(_ => { InitPerksTab(); }).AddTo(ActivateDisposables);
            _dataCentralService.PumpingDataModel.MaxStickmanLevel.Subscribe(maxLevel =>
            {
                InitStickmanTab();
            }).AddTo(ActivateDisposables);
        }

        private void InitPerksTab()
        {
            InitPerksUIItems();
            ActivatePerksTab();
        }

        private void InitStickmanTab()
        {
            _shopDisposable.Clear();
            _buyButtonDisposable.Clear();
            InitStickmanUIItems();
            ActivateStickmanTab();
            
        }

        private void InitStickmanUIItems()
        {
            ClearAllChildUnderGO(_scrollContentTransform);
            _stickManUIItems.Clear();
            if (_mergeController != null)
            {
                foreach (var stickmanStatsConfig in _configManager.UnitsStatsSo.StickmanUnitsStatsConfigs)
                {
                    StickManUIItem stickman = Instantiate(_stickManUIItem, _scrollContentTransform);
                    stickman.Init(_mergeController, stickmanStatsConfig,
                        _configManager.PrefabsUnitsSO.PlayerUnitPrefabs[stickmanStatsConfig.UnitType],_player);
                    _stickManUIItems.Add(stickmanStatsConfig.UnitType,stickman);
                    OpenLevelsLess(stickmanStatsConfig, stickman,_player.Pumping.GamePerks[PerkTypesEnum.DecreasePrice]);
                    OpenFirstLevelUnit(stickmanStatsConfig, stickman,_player.Pumping.GamePerks[PerkTypesEnum.DecreasePrice]);
                   
                }
            }
        }

        private void OpenLevelsLess(StickmanStatsConfig stickmanStatsConfig, StickManUIItem stickman,
            PumpingPerkData pumpingGamePerk)
        {
            if (/*stickmanStatsConfig.UnitType != PlayerUnitTypeEnum.One &&*/(int) stickmanStatsConfig.UnitType <= (int) (_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3))
            {
                stickman.BuyButton.IsInteractable = true;
                stickman.LockTemplate.gameObject.SetActive(false);
                SubscribeToBuyEvent(stickmanStatsConfig,stickman,pumpingGamePerk);
            }
            else
            {
                stickman.BuyButton.IsInteractable = false;
                stickman.LockTemplate.LockLabel.text = $"{ScriptLocalization.Buttons_Shop.UnlockCannon} {stickmanStatsConfig.Level + 3}";
                stickman.LockTemplate.gameObject.SetActive(true);
            }
        }

        private void OpenFirstLevelUnit(StickmanStatsConfig stickmanStatsConfig, StickManUIItem stickman,
            PumpingPerkData pumpingGamePerk)
        {
            if ((int) stickmanStatsConfig.UnitType == (int) PlayerUnitTypeEnum.One)
            {
                stickman.BuyButton.IsInteractable = true;
                stickman.LockTemplate.gameObject.SetActive(false);
                SubscribeToBuyEvent(stickmanStatsConfig,stickman,pumpingGamePerk);
            }
        }

        private void SubscribeToBuyEvent(StickmanStatsConfig stickmanStatsConfig, StickManUIItem stickman,
            PumpingPerkData pumpingGamePerk)
        {
            if (stickmanStatsConfig.UnitType == _dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3 && _showAd)
            {
                stickman.ActiveAdButton();
                _buyButtonDisposable.Clear();
                stickman.FreeUnitBtn.OnClickAsObservable.Subscribe(_ =>
                {
                    OnRewardClaim(stickmanStatsConfig,stickman,pumpingGamePerk);
                }).AddTo(_buyButtonDisposable);
            }
            stickman.BuyButton.OnClickAsObservable.Subscribe(_ => { BuyStickman(stickmanStatsConfig,stickman,pumpingGamePerk); })
                .AddTo(_buyButtonDisposable);
           
            
        }

        private void BuyStickman(StickmanStatsConfig stickmanStatsConfig, StickManUIItem stickman,
            PumpingPerkData pumpingGamePerk)
        {
            int price = (int) (stickmanStatsConfig.Price * (1 - pumpingGamePerk.Value / 100));
            if (price <= _dataCentralService.StatsDataModel.CoinsCount.Value)
            {
                stickman.AddStickmanToPlayGround();
                _dataCentralService.StatsDataModel.MinusCoinsCount(price);
            }
            else
            {
                _manager.GetWindow<PopupMessageWindow>().Init(ScriptLocalization.Messages.WarningTitle,ScriptLocalization.Messages.NotEnoughFunds);
                _manager.Show<PopupMessageWindow>();
            }
        }

        private void AddFreeUnitForAdWatch()
        {
            
        }

        private void InitPerksUIItems()
        {
            ClearAllChildUnderGO(_scrollContentTransform);
            if (_mergeController != null)
            {
                foreach (var perkType in _configManager.PrefabsUnitsSO.PerkIcons.Keys)
                {
                    PerkUIItem perk = Instantiate(_perkUIItem, _scrollContentTransform);
                    perk.Init(_player.Pumping.GamePerks[perkType], _configManager, perkType);
                   _player.Pumping.GamePerks.ObserveReplace().Subscribe(_ =>
                       {
                           perk.Init(_player.Pumping.GamePerks[perkType], _configManager, perkType);
                           
                       })
                       .AddTo(_shopDisposable);
                    SubscribeToPerkUpgrade(perk, perkType, _player.Pumping.GamePerks[perkType]);
                }
            }
        }

        private void SubscribeToPerkUpgrade(PerkUIItem perk, PerkTypesEnum perkType,PumpingPerkData pumpingPerkData)
        {
            _dataCentralService.StatsDataModel.CoinsCount.Subscribe(_ =>
            {
                perk.BuyButton.IsInteractable = (_ >= pumpingPerkData.Cost);
            }).AddTo(_shopDisposable);
            perk.BuyButton.OnClickAsObservable.Subscribe(_ =>
                {
                    if (_dataCentralService.StatsDataModel.CoinsCount.Value >= pumpingPerkData.Cost )
                    {
                        _player.Pumping.UpgradeGamePerk(perkType);
                        _dataCentralService.StatsDataModel.MinusCoinsCount( pumpingPerkData.Cost);
                    }
                }).AddTo(_shopDisposable);
          
        }

        private void ClearAllChildUnderGO(Transform tranformGO)
        {
            while (tranformGO.childCount > 0)
            {
                DestroyImmediate(tranformGO.GetChild(0).gameObject);
            }
        }

        private void ActivateStickmanTab()
        {
            _stickmanShopBtn.TargetImage.color = Color.blue;
            _perksShopBtn.TargetImage.color = Color.white;
        }

        private void ActivatePerksTab()
        {
            _stickmanShopBtn.TargetImage.color = Color.white;
            _perksShopBtn.TargetImage.color = Color.blue;
        }

        protected override void OnDeactivate()
        {
            _shopDisposable.Clear();
            _buyButtonDisposable.Clear();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Play);
        }
       
        private void OnRewardClaim(StickmanStatsConfig stickmanStatsConfig, StickManUIItem stickman,
            PumpingPerkData pumpingGamePerk)
        {
            _iapService.ShowRewardedBreak(value =>
            {
                if (value)
                {
                    StickManUIItem stickman =
                        _stickManUIItems[_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3];
                    stickman.AddStickmanToPlayGround();
                    stickman.ActivateCommonButton();
                    _buyButtonDisposable.Clear();
                    stickman.BuyButton.OnClickAsObservable.Subscribe(_ => { BuyStickman(stickmanStatsConfig,stickman,pumpingGamePerk); })
                        .AddTo(_buyButtonDisposable);
                    _showAd = false;
                    AlertUnitsTab(false);
                }    
            });
        }

        public void StartAdTimer()
        {   
            if(_timerModel != null) _timerModel.StopTick();
            _timerModel = _timerService.AddGameTimer(_adTimerCooldown, f => { }, () =>
            {
                _showAd = true;
                AlertUnitsTab(true);
               
            });
        }

        public void AlertUnitsTab(bool value)
        {
            _unitsAlert.Value = value;
            _stickmanShopBtnAlert.SetActive(value);
        }

        public void AlertPerksTab(bool value)
        {
            _perksAlert.Value = value;
            _perksShopBtnAlert.SetActive(value);
        }

    }
}