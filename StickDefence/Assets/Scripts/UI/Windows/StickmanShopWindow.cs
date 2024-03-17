using Models.Merge;
using TonkoGames.Controllers.Core;
using UI.Common;
using UI.Content.Shop;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Windows
{
    public class StickmanShopWindow:Window
    {
        [Header("StickMan UI Item")]
        [SerializeField] private StickManUIItem _stickManUIItem;

        [Header("Scroll Content Transform")]
        [SerializeField] private Transform _scrollContentTransform;

        [Header("Close window Btn")] 
        [SerializeField] private UIButton _closeWindowBtn;
        
        [Inject] private ConfigManager _configManager;

        public WindowPriority Priority = WindowPriority.AboveTopPanel;
        private IPlaceableUnit _mergeController; 
        public void Init(IPlaceableUnit mergeController)
        {
            _mergeController = mergeController;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            InitWindowButtons();
            InitStickmanUIItems();
        }

        private void InitWindowButtons()
        {
            _closeWindowBtn.OnClickAsObservable.Subscribe(_ =>
            {
                _manager.Hide(this);
            }).AddTo(ActivateDisposables);
        }

        private void InitStickmanUIItems()
        {
            if (_mergeController != null)
            {
                foreach (var stickmanStatsConfig in _configManager.StickmanUnitsSO.StickmanStatsConfigs)
                {
                    StickManUIItem stickman = Instantiate(_stickManUIItem, _scrollContentTransform);
                    stickman.Init(_mergeController, ActivateDisposables, stickmanStatsConfig);
                }
            }
        }
    }
}