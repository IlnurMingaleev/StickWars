using System;
using System.Collections.Generic;
using TonkoGames.Controllers.Core;
using Cysharp.Threading.Tasks;
using Models.DataModels;
using Models.IAP.InApps.InAppRewards;
using UI.UIManager;
using UI.Windows;
using UnityEngine;

namespace Models.IAP.InApps
{
    public class InAppSystem
    {
        private readonly IWindowManager _windowManager;
        private readonly ConfigManager _configManager;
        private readonly IDataCentralService _dataCentralService;
        private readonly IAPService _iapService;
        
        private List<IInAppRewardModel> _freeInApps = new List<IInAppRewardModel>();
        private List<string> _notConsumed = new List<string>();
        
        
        public InAppSystem(IWindowManager windowManager, ConfigManager configManager, 
            IDataCentralService dataCentralService, IAPService iapService)
        {
            _windowManager = windowManager;
            _configManager = configManager;
            _dataCentralService = dataCentralService;
            _iapService = iapService;
        }
        
        public void AddInAppReward(PaymentProductEnum paymentProduct)
        {
            GetInAppReward(paymentProduct).Collect();
        }
        
#if UNITY_WEBGL || UNITY_EDITOR
        public void PreLoadInApps(List<GamePush.FetchPlayerPurchases> arg0)
        {
            foreach (var playerProduct in arg0)
            {
                // if (playerProduct.gift)
                // {
                //     _freeInApps.Add(GetInAppReward((PaymentProductEnum) Enum.Parse(typeof(PaymentProductEnum), playerProduct.tag, true)));
                // }
                // else
                // {
                //     _notConsumed.Add((PaymentProductEnum) Enum.Parse(typeof(PaymentProductEnum), playerProduct.tag, true));
                // }
                _notConsumed.Add(playerProduct.tag);
            }
        }
#endif
        private IInAppRewardModel GetInAppReward(PaymentProductEnum paymentProduct)
        {
            switch (paymentProduct)
            {
                case PaymentProductEnum.Purchase_Gem1:
                case PaymentProductEnum.Purchase_Gem2:
                case PaymentProductEnum.Purchase_Gem3:
                case PaymentProductEnum.Purchase_Gem4:
                case PaymentProductEnum.Purchase_Gem5:
                    return new InAppGemRewardModel(_windowManager, _configManager, _dataCentralService, paymentProduct);
                case PaymentProductEnum.RemoveADS:
                    return new InAppADSRemove(_dataCentralService);
            }

            return default;
        }

        public async UniTaskVoid TryCollectNonCollectedInApps()
        {
            // if (_freeInApps.Count > 0)
            // {
            //     _freeInApps[0].Collect();
            //     _freeInApps.RemoveAt(0);
            //     return; 
            // }

            await UniTask.DelayFrame(10);
            
            if (_notConsumed.Count > 0)
            {
                _iapService.PaymentModel.Consuming(_notConsumed[0]);
                _notConsumed.RemoveAt(0);
            }
        }
    }
}