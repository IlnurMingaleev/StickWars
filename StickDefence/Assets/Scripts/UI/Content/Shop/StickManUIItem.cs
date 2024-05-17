using System;
using Enums;
using Models.Merge;
using Models.Player;
using Models.Player.PumpingFragments;
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
        [SerializeField] private TMP_Text _buyBtnLabel;

        [Header("StickManIcon")] 
        [SerializeField] private Image _stickmanImage;

        [Header("Buy Conditions")]
        [SerializeField] private TMP_Text _damageLabel;

        [Header("LockTemplate")]
        [SerializeField] private LockTemplate _lockTemplate;

        [Header("FreeUnitBtn")] 
        [SerializeField] private UIButton _freeUnitBtn;
        
        public UIButton BuyButton => _buyBtn;
        public UIButton FreeUnitBtn => _freeUnitBtn;
        public LockTemplate LockTemplate => _lockTemplate;
        private IPlaceableUnit _mergeController;
        private StickmanStatsConfig _statsConfig;
        private PumpingPerkData _perkData;
        private CompositeDisposable _disposable = new CompositeDisposable();
        public void Init(IPlaceableUnit mergeController, StickmanStatsConfig stickmanStatsConfig, PlayerPrefabModel playerPrefabModel,IPlayer player)
        {
            _statsConfig =stickmanStatsConfig ;
            _mergeController = mergeController;
            _stickmanUnitType = stickmanStatsConfig.UnitType;
            _levelLabel.text = stickmanStatsConfig.Level.ToString();
           // _stickmanImage.sprite = playerPrefabModel.uiIcon;
             PumpingPerkData perkDataDamage = player.Pumping.GamePerks[PerkTypesEnum.RecruitsDamage];
             PumpingPerkData perkData = player.Pumping.GamePerks[PerkTypesEnum.DecreasePrice];
            _damageLabel.text = $"Damage: {(int)(stickmanStatsConfig.Damage*(1+perkDataDamage.Value/100))}";
            _buyBtnLabel.text = $"Buy: {(int)(stickmanStatsConfig.Price *(1 - (perkData.Value/100)))}";;
           

        }

        public void ActiveAdButton()
        {
            _buyBtn.gameObject.SetActive(false);
            _freeUnitBtn.gameObject.SetActive(true);
        }

        public void ActivateCommonButton()
        {
            _freeUnitBtn.gameObject.SetActive(false);
            _buyBtn.gameObject.SetActive(true);
        }

        public void AddStickmanToPlayGround()
        {
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