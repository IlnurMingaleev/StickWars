using System;
using Enums;
using Models.Merge;
using Models.SO.Core;
using Models.SO.Visual;
using TMPro;
using TonkoGames.Controllers.Core;
using UI.Common;
using UniRx;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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

        public UIButton BuyButton => _buyBtn;
        
        private IPlaceableUnit _mergeController;
        private StickmanStatsConfig _statsConfig;
        private CompositeDisposable _disposable = new CompositeDisposable();
        public void Init(IPlaceableUnit mergeController, StickmanStatsConfig stickmanStatsConfig, PlayerPrefabModel playerPrefabModel)
        {
            _statsConfig =stickmanStatsConfig ;
            _mergeController = mergeController;
            _stickmanUnitType = stickmanStatsConfig.UnitType;
            _levelLabel.text = stickmanStatsConfig.Level.ToString();
            _stickmanImage.sprite = playerPrefabModel.uiIcon;
            /*_buyBtn.OnClickAsObservable.Subscribe(_=>
            {
                AddStickmanToPlayGround();
            }).AddTo(_disposable);*/
           
        }

        public void AddStickmanToPlayGround()
        {
            Debug.LogWarning("Stickman Created");
            if (_mergeController != null)
            {
                _mergeController.PlaceDefinedItem((int)_stickmanUnitType);
            }
        }

        private void OnDisable()
        {
            _disposable.Clear();
        }
    }
}