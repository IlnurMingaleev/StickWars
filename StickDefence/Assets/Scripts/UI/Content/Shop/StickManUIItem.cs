using System;
using Enums;
using Models.Merge;
using Models.SO.Core;
using TMPro;
using UI.Common;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Content.Shop
{
    public class StickManUIItem :MonoBehaviour
    {
        [Header("Stickman Type")] 
        private PlayerUnitTypeEnum _stickmanUnitType;
        [Header("Level")] 
        [SerializeField] private TMP_Text _levelLabel;

        [Header("Buy Button")]
        [SerializeField] private UIButton _buyBtn;

        [Header("StickManIcon")] 
        [SerializeField] private Image _stickmanImage;

        [Header("Buy Conditions")]
        [SerializeField] private TMP_Text _buyConditionsLabel;

        private IPlaceableUnit _mergeController;
        private StickmanStatsConfig _statsConfig;
        public void Init(IPlaceableUnit mergeController, CompositeDisposable activateDisposable,
            StickmanStatsConfig stickmanStatsConfig)
        {
            _statsConfig = stickmanStatsConfig;
            _mergeController = mergeController;
            _stickmanUnitType = stickmanStatsConfig.UnitType;
            _levelLabel.text = stickmanStatsConfig.Level.ToString();
            _stickmanImage.sprite = stickmanStatsConfig.uiIcon;
            _buyBtn.OnClickAsObservable.Subscribe(_=>
            {
                AddStickmanToPlayGround();
            }).AddTo(activateDisposable);
           
        }

        private void AddStickmanToPlayGround()
        {
            Debug.LogWarning("Stickman Created");
            if (_mergeController!= null)
            {
                _mergeController.PlaceDefinedItem((int)_stickmanUnitType);
            }
        }
    }
}