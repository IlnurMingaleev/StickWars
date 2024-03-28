using System;
using Enums;
using I2.Loc;
using Models.Controllers;
using Models.Player;
using Models.Player.PumpingFragments;
using TMPro;
using Tools.Extensions;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Content.UpgradeBlock
{
    public class UIPerkUpgrade : UIBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private PerkTypesEnum _perkType;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TMP_Text _costLabel;
        [SerializeField] private GameObject _maxBlock;
        [SerializeField] private UIButton _buttonBuy;
        [SerializeField] private Image _rayCastTarget;
        [SerializeField] private CurrencyIconBlock _currencyIconBlock;

        [Inject] private readonly IPlayer _player;

        private CompositeDisposable _disposable = new CompositeDisposable();
        
        public PerkTypesEnum PerkType => _perkType;
        private CurrencyTypeEnum _currencyTypeEnum;
        
        public event Action<UIPerkUpgrade> PurchasedLevelUp;

        private int _costUpgrade = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            LocalizationManager.OnLocalizeEvent += SetNameLocalization;
            SetNameLocalization();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _disposable.Clear();
            LocalizationManager.OnLocalizeEvent -= SetNameLocalization;
        }

        public void Init(PumpingPerkData pumpingPerkData)
        {
            _costUpgrade = pumpingPerkData.Cost;
            SetValue(pumpingPerkData.Value);

            if (pumpingPerkData.IsMaxLevel)
            {
                _maxBlock.SetActive(true);
                _currencyIconBlock.gameObject.SetActive(false);
                _costLabel.gameObject.SetActive(false);
                _rayCastTarget.raycastTarget = false;
            }
            else
            {
                _disposable.Clear();
                _rayCastTarget.raycastTarget = true;
                _maxBlock.SetActive(false);
                _currencyIconBlock.gameObject.SetActive(true);
                _costLabel.gameObject.SetActive(true);
                _costLabel.text = SetScoreExt.ConvertIntToStringValue(pumpingPerkData.Cost, 1);
                _currencyIconBlock.SetPerType(pumpingPerkData.CurrencyType);
                _player.SubscribeToCurrencyBuyType(pumpingPerkData.CurrencyType).Subscribe(CurrencyChange).AddTo(_disposable);
                _buttonBuy.OnClickAsObservable.Subscribe(_ => OnBuy()).AddTo(_disposable);
                _currencyTypeEnum = pumpingPerkData.CurrencyType;
            }
        }
        
        private void SetNameLocalization()
        {
            _nameLabel.text = _perkType.ToTranslatedName();
        }

        private void OnBuy()
        {
            PurchasedLevelUp?.Invoke(this);
            _player.ChangeCurrencyBuyType(_currencyTypeEnum, _costUpgrade);
        }

        private void CurrencyChange(int currency)
        {
            _buttonBuy.IsInteractable = currency >= _costUpgrade;
        }

        private void SetValue(float value)
        {
            switch (_perkType)
            {
                /*case PerkTypesEnum.Defense:
                case PerkTypesEnum.CriticalChance:
                case PerkTypesEnum.CriticalMultiplier:
                    _valueLabel.text = $"{value * 100}%";
                    break;*/
                default:
                    _valueLabel.text = SetScoreExt.ConvertIntToStringValue(value, 1);
                    break;
            }
        }
    }
}