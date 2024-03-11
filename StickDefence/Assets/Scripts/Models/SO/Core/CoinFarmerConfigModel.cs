using System;

namespace Models.SO.Core
{
    [Serializable]
    public struct CoinFarmerConfigModel
    {
        public int CoinSmeltingGoldTick;
        public int CoinSmeltingCost;
        public int StorageCapacity;
        public int StorageCost;
    }
}