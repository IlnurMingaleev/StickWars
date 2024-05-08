using System;
using Enums;
using I2.Loc;
using Models.Player;
using Models.Player.PumpingFragments;
using TMPro;
using Tools.Extensions;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.Battle
{
    public class UIBattleUpgradePerk : UIBehaviour
    {
        [SerializeField] private PerkTypesEnum _perkType;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TMP_Text _costLabel;
        [SerializeField] private CurrencyIconBlock _currencyIconBlock;
        [SerializeField] private GameObject _maxBlock;
        [SerializeField] private UIButton _buttonBuy;

        [Inject] private readonly IPlayer _player;

        private CompositeDisposable _disposable = new CompositeDisposable();
        
        public PerkTypesEnum PerkType => _perkType;
        private CurrencyTypeEnum _currencyTypeEnum;
        
        public event Action<UIBattleUpgradePerk> PurchasedLevelUp;

        private int _costUpgrade = 0;

        protected override void OnDisable()
        {
            base.OnDisable();
            _disposable.Clear();
        }

        public void Init(PumpingPerkData pumpingPerkData)
        {
            _costUpgrade = pumpingPerkData.Cost;
            SetValue(pumpingPerkData.Value);
            _buttonBuy.IsInteractable = !pumpingPerkData.IsMaxLevel;
            _currencyIconBlock.gameObject.SetActive(!pumpingPerkData.IsMaxLevel);
            _costLabel.gameObject.SetActive(!pumpingPerkData.IsMaxLevel);
            _maxBlock.SetActive(pumpingPerkData.IsMaxLevel);
            
            if (!pumpingPerkData.IsMaxLevel)
            {
                _disposable.Clear();
                
                _costLabel.text = SetScoreExt.ConvertIntToStringValue(pumpingPerkData.Cost, 1);
                _currencyIconBlock.SetPerType(pumpingPerkData.CurrencyType);
                _player.SubscribeToCurrencyBuyType(pumpingPerkData.CurrencyType).Subscribe(CurrencyChange).AddTo(_disposable);
                _buttonBuy.OnClickAsObservable.Subscribe(_ => OnBuy()).AddTo(_disposable);
                _currencyTypeEnum = pumpingPerkData.CurrencyType;
            }
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
                    _valueLabel.text = SetScoreExt.ConvertIntToStringValue(value, 2);
                    break;
            }
        }
    }
}