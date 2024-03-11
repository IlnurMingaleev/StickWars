using TonkoGames.Controllers.Core;
using I2.Loc;
using Models.IAP;
using Models.IAP.InApps;
using TMPro;
using Tools.Extensions;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.Shop
{
    public class PaymentBlock : UIBehaviour
    {
        [SerializeField] protected PaymentProductEnum _paymentProductType;
        [SerializeField] protected TMP_Text _priceLabel;
        [SerializeField] protected TMP_Text _valueLabel;
        [SerializeField] private UIButton _purchaseButton;
        
        protected InAppSettings _inAppSettings;

        private CompositeDisposable _disposable = new CompositeDisposable();
        
        [Inject] protected readonly IIAPService _iapService;
        [Inject] protected readonly ConfigManager _ConfigManager;

        protected override void Awake()
        {
            base.Awake();
            if (!_iapService.PaymentModel.InAppSettings.ContainsKey(_paymentProductType))
            {
                gameObject.SetActive(false);
            }
            else
            {
                _inAppSettings = _iapService.PaymentModel.InAppSettings[_paymentProductType];
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _purchaseButton.OnClickAsObservable.Subscribe(_ => OnPurchase()).AddTo(_disposable);
            LocalizationManager.OnLocalizeEvent += OnLocalizeEvent;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _disposable.Clear();
            LocalizationManager.OnLocalizeEvent -= OnLocalizeEvent;
        }

        protected virtual void OnPurchase()
        {
            _iapService.PaymentModel.Purchase(_paymentProductType);
        }
        protected virtual void OnLocalizeEvent()
        {
        }
    }
}