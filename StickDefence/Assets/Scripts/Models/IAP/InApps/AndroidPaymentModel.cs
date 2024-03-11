using Models.DataModels;

namespace Models.IAP.InApps
{
    public class AndroidPaymentModel : PaymentModel
    {
        public AndroidPaymentModel(IAPService iapService, IDataCentralService dataCentralService,
            InAppSystem inAppSystem) : base(iapService, dataCentralService, inAppSystem)
        {
        }
    }
}