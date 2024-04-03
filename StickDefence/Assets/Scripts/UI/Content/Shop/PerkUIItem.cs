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
    public class PerkUIItem :MonoBehaviour
    {
        [Header("Stickman Type")] 
        private PlayerUnitTypeEnum _stickmanUnitType;
        [Header("Level")] 
        [SerializeField] private TMP_Text _levelLabel;

        [Header("Buy Button")]
        [SerializeField] private UIButton _buyBtn;
        [SerializeField] private TMP_Text _buyBtnLabel;

        [Header("StickManIcon")] 
        [SerializeField] private Image _perkImage;

        [Header("Buy Conditions")]
        [SerializeField] private TMP_Text _buyConditionsLabel;

        public UIButton BuyButton => _buyBtn;
        private IPlaceableUnit _mergeController;
        private CompositeDisposable _disposable = new CompositeDisposable();
        private ConfigManager _configManager;
        public void Init(PlayerPerkConfigModel playerPerk, ConfigManager configManager, PerkTypesEnum perkType, float cost, int perkLevel)
        {
            _configManager = configManager;
            _perkImage.sprite = _configManager.PrefabsUnitsSO.PerkIcons[perkType].PerkIcon;
            _buyBtnLabel.text = $"{cost}";
            _levelLabel.text = $"{perkLevel}";
        }

        private void OnDisable()
        {
            _disposable.Clear();
        }
    }
}