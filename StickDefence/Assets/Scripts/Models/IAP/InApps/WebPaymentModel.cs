using System;
using System.Collections.Generic;
using Models.DataModels;
using UnityEngine;

namespace Models.IAP.InApps
{
#if UNITY_WEBGL || UNITY_EDITOR
    using GamePush;
    public class WebPaymentModel : PaymentModel
    {
        private List<FetchPlayerPurchases> _playerNotConsumedPurchases;
        public override bool IsPaymentsAvailable => GP_Payments.IsPaymentsAvailable();
        
        public WebPaymentModel(IAPService iapService, IDataCentralService dataCentralService,
            InAppSystem inAppSystem) : base(iapService, dataCentralService, inAppSystem)
        {
        }
        
        public override void Fetch()
        {
            GP_Payments.OnFetchProducts += OnFetchProducts;
            GP_Payments.OnFetchPlayerPurchases += PlayerPurchases;
            GP_Payments.Fetch();
        }

        private void PlayerPurchases(List<FetchPlayerPurchases> arg0)
        {
            GP_Payments.OnFetchPlayerPurchases -= PlayerPurchases;
            _InAppSystem.PreLoadInApps(arg0);
        }

        private void OnFetchProducts(List<FetchProducts> fetchProducts)
        {
            foreach (var product in fetchProducts)
            {
                if (Enum.TryParse<PaymentProductEnum>(product.tag, true, out PaymentProductEnum result))
                    _InAppSettings.Add(result, new InAppSettings
                    {
                        Type = result,
                        Price = product.price,
                        RealCurrencyType = (RealCurrencyEnum) Enum.Parse(typeof(RealCurrencyEnum), product.currency, true)
                    });
            }

            InAppsFetched = true;
        }
        
        public override bool Purchase(PaymentProductEnum productEnum)
        {
            if (!_IapService.IapModel.IsLogin)
            {
                _IapService.IapModel.TryLogin();
                return false;
            }
            
            _IapService.SetBlocker(true);
            GP_Payments.Purchase(productEnum.ToString(), PurchaseSuccess, PurchaseError);
            return true;
        }
        
        private void PurchaseSuccess(string id)
        {
            GP_Payments.Consume(id, ConsumeSuccess, ConsumeError);
        }

        public override void Consuming(string productTag)
        {
            GP_Payments.Consume(productTag, id => _InAppSystem.AddInAppReward((PaymentProductEnum)Enum.Parse(typeof(PaymentProductEnum), id, true)));
        }
        
        private void PurchaseError()
        {
            _IapService.SetBlocker(false);
        }

        private void ConsumeSuccess(string id)
        {
            _IapService.SetBlocker(false);
            _InAppSystem.AddInAppReward((PaymentProductEnum)Enum.Parse(typeof(PaymentProductEnum), id, true));
        }
        
        private void ConsumeError()
        {
            _IapService.SetBlocker(false);
        }
    }
#endif
}