using System;
using Models.DataModels;
using UniRx;
using UnityEngine;

namespace Models.IAP.InApps
{
    public class DevPaymentModel : PaymentModel
    {
        public override bool IsPaymentsAvailable => true;
        public DevPaymentModel(IAPService iapService, IDataCentralService dataCentralService,
            InAppSystem inAppSystem) : base(iapService, dataCentralService, inAppSystem)
        {
        }
        
        public override void Fetch()
        {
            _InAppSettings.Add(PaymentProductEnum.Purchase_Gem1, new InAppSettings
            {
                Type = PaymentProductEnum.Purchase_Gem1,
                Price = 10,
                RealCurrencyType = RealCurrencyEnum.DOLLAR
            });
            
            _InAppSettings.Add(PaymentProductEnum.Purchase_Gem2, new InAppSettings
            {
                Type = PaymentProductEnum.Purchase_Gem2,
                Price = 20,
                RealCurrencyType = RealCurrencyEnum.DOLLAR
            });
            
            _InAppSettings.Add(PaymentProductEnum.Purchase_Gem3, new InAppSettings
            {
                Type = PaymentProductEnum.Purchase_Gem3,
                Price = 30,
                RealCurrencyType = RealCurrencyEnum.DOLLAR
            });
            
            _InAppSettings.Add(PaymentProductEnum.Purchase_Gem4, new InAppSettings
            {
                Type = PaymentProductEnum.Purchase_Gem4,
                Price = 40,
                RealCurrencyType = RealCurrencyEnum.DOLLAR
            });
            
            _InAppSettings.Add(PaymentProductEnum.Purchase_Gem5, new InAppSettings
            {
                Type = PaymentProductEnum.Purchase_Gem5,
                Price = 50,
                RealCurrencyType = RealCurrencyEnum.DOLLAR
            });
            
            _InAppSettings.Add(PaymentProductEnum.RemoveADS, new InAppSettings
            {
                Type = PaymentProductEnum.RemoveADS,
                Price = 10,
                RealCurrencyType = RealCurrencyEnum.DOLLAR
            });
            
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
            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => EndPurchase(productEnum));
            return true;
        }

        private void EndPurchase(PaymentProductEnum productEnum)
        {
            _IapService.SetBlocker(false);
            _InAppSystem.AddInAppReward(productEnum);
        }
    }
}