using Models.Merge;
using TonkoGames.Controllers.Core;
using UI.Content.Shop;
using UI.UIManager;
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
        [Inject] private ConfigManager _configManager;
        

        protected override void OnActivate()
        {
            base.OnActivate();

            InitStickmanUIItems();
        }

        private void InitStickmanUIItems()
        {
            while (_scrollContentTransform.childCount != 0)
            {
                Destroy(_scrollContentTransform.GetChild(0));
            }
            MergeController mergeController = FindObjectOfType<MergeController>();
            foreach (var stickmanStatsConfig in _configManager.StickmanUnitsSO.StickmanStatsConfigs)
            {
                StickManUIItem stickman = Instantiate(_stickManUIItem, _scrollContentTransform);
                stickman.Init(mergeController.GetComponent<IPlaceableUnit>(), ActivateDisposables, stickmanStatsConfig);
            }
        }
    }
}