using Models.Merge;
using Models.Player;
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

            [Header("Scroll Content Transform")] [SerializeField]
        private Transform _scrollContentTransform;

        [Header("Close window Btn")] [SerializeField]
        private UIButton _closeWindowBtn;

        [Header("Tab Buttons")]
        [SerializeField] private UIButton _stickmanShopBtn;
        [SerializeField] private UIButton _perksShopBtn;

        [Inject] private ConfigManager _configManager;
        [Inject] private IPlayer _player;

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
            InitStickmanTab();
        }

        private void InitWindowButtons()
        {
            _closeWindowBtn.OnClickAsObservable.Subscribe(_ => { _manager.Hide(this); }).AddTo(ActivateDisposables);
            _stickmanShopBtn.OnClickAsObservable.Subscribe(_ => { InitStickmanTab(); }).AddTo(ActivateDisposables);
            _perksShopBtn.OnClickAsObservable.Subscribe(_ => { InitPerksTab(); }).AddTo(ActivateDisposables);
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
                    stickman.BuyButton.OnClickAsObservable.Subscribe(_ => { stickman.AddStickmanToPlayGround(); })
                        .AddTo(_shopDisposable);
                }
            }
        }

        private void InitPerksUIItems()
        {
            ClearAllChildUnderGO(_scrollContentTransform);
            if (_mergeController != null)
            {
                foreach (var perkType in _configManager.PrefabsUnitsSO.PerkIcons.Keys)
                {
                    PerkUIItem perk = Instantiate(_perkUIItem, _scrollContentTransform);
                    perk.Init(_configManager.PumpingConfigSo.GamePerks[perkType], _configManager, perkType);
                    perk.BuyButton.OnClickAsObservable.Subscribe(_ =>
                        {
                            _player.Pumping.UpgradeGamePerk(perkType);
                        })
                        .AddTo(_shopDisposable);
                }
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