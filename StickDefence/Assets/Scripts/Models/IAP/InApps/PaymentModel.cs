using System;
using System.Collections.Generic;
using Models.DataModels;

namespace Models.IAP.InApps
{
    public interface IPaymentModel
    {
        public bool IsPaymentsAvailable { get; }
        public IReadOnlyDictionary<PaymentProductEnum, InAppSettings> InAppSettings { get; }
        public bool InAppsFetched { get; }
        public void Fetch();
        bool Purchase(PaymentProductEnum productEnum);
        void Consuming(string tag);

    }
    public abstract class PaymentModel : IPaymentModel
    {
        public virtual bool IsPaymentsAvailable => false;
        
        protected readonly IAPService _IapService;
        protected readonly IDataCentralService _DataCentralService;
        protected readonly InAppSystem _InAppSystem;

        protected Dictionary<PaymentProductEnum, InAppSettings> _InAppSettings =
            new Dictionary<PaymentProductEnum, InAppSettings>();

        public IReadOnlyDictionary<PaymentProductEnum, InAppSettings> InAppSettings => _InAppSettings;

        public bool InAppsFetched { get; protected set; }

        protected PaymentModel(IAPService iapService, IDataCentralService dataCentralService, InAppSystem inAppSystem)
        {
            _IapService = iapService;
            _DataCentralService = dataCentralService;
            _InAppSystem = inAppSystem;
        }

        public virtual void Fetch()
        {
        }

        public virtual bool Purchase(PaymentProductEnum productEnum)
        {
            return false;
        }

        public virtual void Consuming(string tag)
        {
        }
    }
}