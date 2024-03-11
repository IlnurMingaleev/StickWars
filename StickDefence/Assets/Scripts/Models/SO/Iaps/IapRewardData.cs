using System;
using Enums;
using Models.IAP;

namespace Models.SO.Iaps
{
    [Serializable]
    public struct IapRewardData
    {
        public IapTypeEnum IapType;
        public int Value;
        public int Cost;
    }
}