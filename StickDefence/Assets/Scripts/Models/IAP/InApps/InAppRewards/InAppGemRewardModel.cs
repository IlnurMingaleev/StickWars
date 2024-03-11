using TonkoGames.Controllers.Core;
using Cysharp.Threading.Tasks;
using Models.DataModels;
using UI.Content.Rewards;
using UI.UIManager;
using UI.Windows;

namespace Models.IAP.InApps.InAppRewards
{
    public class InAppGemRewardModel : IInAppRewardModel
    {
        private readonly IDataCentralService _dataCentralService;
        private readonly ConfigManager _configManager;
        private readonly IWindowManager _windowManager;
        private readonly PaymentProductEnum _paymentProduct;
        
        public InAppGemRewardModel(IWindowManager windowManager, ConfigManager configManager, IDataCentralService dataCentralService, PaymentProductEnum paymentProduct)
        {
            _windowManager = windowManager;
            _configManager = configManager;
            _dataCentralService = dataCentralService;
            _paymentProduct = paymentProduct;
        }
        
        public void Collect()
        {
            _windowManager.Show<RewardCollectWindow>().Collect(new RewardContains()
            {
                Gem = _configManager.IapSO.AppRewards[_paymentProduct].Gem
            }).Forget();
            
            _dataCentralService.StatsDataModel.AddGemsCount(_configManager.IapSO.AppRewards[_paymentProduct].Gem);
            
            _dataCentralService.SaveFull();
            _dataCentralService.Sync();
        }
    }
}