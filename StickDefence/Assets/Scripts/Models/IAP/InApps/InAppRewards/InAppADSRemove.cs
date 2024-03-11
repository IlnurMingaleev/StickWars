using Models.DataModels;

namespace Models.IAP.InApps.InAppRewards
{
    public class InAppADSRemove : IInAppRewardModel
    {
        private readonly IDataCentralService _dataCentralService;
        
        public InAppADSRemove(IDataCentralService dataCentralService)
        {
            _dataCentralService = dataCentralService;
        }
        
        public void Collect()
        {
            _dataCentralService.SubData.SetADSRemove(true);
            _dataCentralService.SaveFull();
            _dataCentralService.Sync();
        }
    }
}