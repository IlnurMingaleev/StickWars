using System;
using Enums;
using Models.IAP.InApps;
using UnityEngine.Serialization;

namespace Models.SO.Iaps
{
    [Serializable]
    public struct InAppRewardConfig
    {
        public PaymentProductEnum PaymentProduct;
        public int Gem;
    }
}