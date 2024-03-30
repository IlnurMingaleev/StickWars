using Enums;
using I2.Loc;
using Models.DataModels;
using Models.DataModels.Data;
using Models.Merge;
using Models.Player;
using Models.SO.Core;
using TMPro;
using TonkoGames.Controllers.Core;
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

        [Inject] private ConfigManager _configManager;
        [Inject] private IPlayer _player;
        [Inject] private IDataCentralService _dataCentralService;

        private WindowPriority Priority = WindowPriority.AboveTopPanel;
        private IPlaceableUnit _mergeController;
        private CompositeDisposable _shopDisposable = new CompositeDisposable();


        public void Init(IPlaceableUnit mergeController)
        {
            _mergeController = mergeController;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            InitWindowButtons();
            InitTextFields();
            InitStickmanTab();
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
            InitStickmanUIItems();
            ActivateStickmanTab();
            
        }

        private void InitStickmanUIItems()
        {
            ClearAllChildUnderGO(_scrollContentTransform);
            if (_mergeController != null)
            {
                foreach (var stickmanStatsConfig in _configManager.UnitsStatsSo.StickmanUnitsStatsConfigs)
                {
                    StickManUIItem stickman = Instantiate(_stickManUIItem, _scrollContentTransform);
                    stickman.Init(_mergeController, stickmanStatsConfig,
                        _configManager.PrefabsUnitsSO.PlayerUnitPrefabs[stickmanStatsConfig.UnitType]);
                    OpenLevelsLess(stickmanStatsConfig, stickman);
                    OpenFirstLevelUnit(stickmanStatsConfig, stickman);
                }
            }
        }

        private void OpenLevelsLess(StickmanStatsConfig stickmanStatsConfig, StickManUIItem stickman)
        {
            if ((int) stickmanStatsConfig.UnitType <= (int) (_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 4))
            {
                stickman.BuyButton.IsInteractable = true;
                SubscribeToBuyEvent(stickman);
            }
            else
            {
                stickman.BuyButton.IsInteractable = false;
                stickman.LockTemplate.LockLabel.text = $"{ScriptLocalization.Buttons_Shop.UnlockCannon} {stickmanStatsConfig.Level + 3}";
                stickman.LockTemplate.gameObject.SetActive(true);
            }
        }

        private void OpenFirstLevelUnit(StickmanStatsConfig stickmanStatsConfig, StickManUIItem stickman)
        {
            if ((int) stickmanStatsConfig.UnitType == (int) PlayerUnitTypeEnum.PlayerOne)
            {
                stickman.BuyButton.IsInteractable = true;
                stickman.LockTemplate.gameObject.SetActive(false);
                SubscribeToBuyEvent(stickman);
            }
        }

        private void SubscribeToBuyEvent(StickManUIItem stickman)
        {
            stickman.BuyButton.OnClickAsObservable.Subscribe(_ => { stickman.AddStickmanToPlayGround(); })
                .AddTo(_shopDisposable);
        }

        private void InitPerksUIItems()
        {
            ClearAllChildUnderGO(_scrollContentTransform);
            if (_mergeController != null)
            {
                foreach (var perkType in _configManager.PrefabsUnitsSO.PerkIcons.Keys)
                {
                    PerkUIItem perk = Instantiate(_perkUIItem, _scrollContentTransform);
                    PlayerPerkConfigModel perkConfigModel = _configManager.PumpingConfigSo.GamePerks[perkType];
                    PerkData perkData = _dataCentralService.PumpingDataModel.PerksReactive[perkType];
                    var nextLevel = perkData.PerkLevel + 1;
                    var perkCost = perkConfigModel.BaseValue + nextLevel * perkConfigModel.AdditionalCost;
                    perk.Init(perkConfigModel, _configManager, perkType, perkCost,perkData.PerkLevel);
                    SubscribeToPerkUpgrade(perk, perkType, perkCost);
                }
            }
        }

        private void SubscribeToPerkUpgrade(PerkUIItem perk, PerkTypesEnum perkType,float perkCost)
        {
            if (_dataCentralService.StatsDataModel.CoinsCount.Value >= perkCost)
            {
                perk.BuyButton.IsInteractable = true;
                perk.BuyButton.OnClickAsObservable.Subscribe(_ =>
                    {
                        _player.Pumping.UpgradeGamePerk(perkType);
                    })
                    .AddTo(_shopDisposable);
            }
            else
            {
                perk.BuyButton.IsInteractable = false;
            }
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
            _stickmanShopBtn.TargetImage.color = Color.green;
            _perksShopBtn.TargetImage.color = Color.white;
        }

        private void ActivatePerksTab()
        {
            _stickmanShopBtn.TargetImage.color = Color.white;
            _perksShopBtn.TargetImage.color = Color.green;
        }

        protected override void OnDeactivate()
        {
            _shopDisposable.Clear();
        }
    }
}