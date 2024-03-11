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
using UI.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Content.UpgradeBlock
{
    public class UISkillUpgrade : UIBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private SkillTypesEnum _skillType;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TMP_Text _costLabel;
        [SerializeField] private GameObject _costBlock;
        [SerializeField] private GameObject _maxBlock;
        [SerializeField] private UIButton _buttonBuy;
        [SerializeField] private Image _rayCastTarget;
        [SerializeField] private CurrencyIconBlock _currencyIconBlock;
        [SerializeField] private UIButton _selectButton;
        [SerializeField] private GameObject _costMaxBlock;
        [SerializeField] private UIButton _faqButton;

        [Inject] private readonly IPlayer _player;
        [Inject] private readonly IWindowManager _windowManager;

        private CompositeDisposable _disposable = new CompositeDisposable();
        private CompositeDisposable _buyDisposable = new CompositeDisposable();
        
        public SkillTypesEnum SkillType => _skillType;
        private CurrencyTypeEnum _currencyTypeEnum;
        
        public event Action<UISkillUpgrade> PurchasedLevelUp;
        public event Action<SkillTypesEnum> SkillSelected;

        private int _costUpgrade = 0;

        private bool _isMax = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            LocalizationManager.OnLocalizeEvent += SetNameLocalization;
            SetNameLocalization();
            _selectButton.OnClickAsObservable.Subscribe(_ => SkillSelected?.Invoke(_skillType)).AddTo(_disposable);
            _faqButton.OnClickAsObservable
                .Subscribe(_ => _windowManager.Show<PopupMessageWindow>(WindowPriority.Dialog).Init(_skillType.ToTranslatedName(), _skillType.ToTranslatedDescription()))
                .AddTo(_disposable);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _disposable.Clear();
            _buyDisposable.Clear();
            LocalizationManager.OnLocalizeEvent -= SetNameLocalization;
        }

        public void SetSelected(bool isSelected)
        {
            _rayCastTarget.raycastTarget = false;
            _costMaxBlock.SetActive(false);
            if (!isSelected)
            {
                _selectButton.gameObject.SetActive(true);
            }
        }

        public void SetNormalState()
        {
            _rayCastTarget.raycastTarget = !_isMax;
            _selectButton.gameObject.SetActive(false);
            _costMaxBlock.SetActive(true);
        }
        public void Init(PumpingSkillData pumpingSkillData)
        {
            _costUpgrade = pumpingSkillData.Cost;
            SetValue(pumpingSkillData.Value);
            _isMax = pumpingSkillData.IsMaxLevel;

            if (pumpingSkillData.IsMaxLevel)
            {
                _maxBlock.SetActive(true);
                _costBlock.SetActive(false);
                _rayCastTarget.raycastTarget = false;
            }
            else
            {
                _buyDisposable.Clear();
                _rayCastTarget.raycastTarget = true;
                _maxBlock.SetActive(false);
                _costBlock.SetActive(true);
                _costLabel.text = SetScoreExt.ConvertIntToStringValue(_costUpgrade, 1);
                _currencyIconBlock.SetPerType(pumpingSkillData.CurrencyType);
                _player.SubscribeToCurrencyBuyType(pumpingSkillData.CurrencyType).Subscribe(CurrencyChange).AddTo(_buyDisposable);
                _buttonBuy.OnClickAsObservable.Subscribe(_ => OnBuy()).AddTo(_buyDisposable);
                _currencyTypeEnum = pumpingSkillData.CurrencyType;
            }
        }
        
        private void SetNameLocalization()
        {
            _nameLabel.text = _skillType.ToTranslatedName();
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
            _valueLabel.text = SetScoreExt.ConvertIntToStringValue(value, 1);
            // switch (_skillType)
            // {
            //     case PerkTypesEnum.Defense:
            //         _valueLabel.text = $"{value}%";
            //         break;
            //     default:
            //         _valueLabel.text = SetScoreExt.ConvertIntToStringValue(value, 1);
            //         break;
            // }
        }
    }
}