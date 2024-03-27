using System;
using Enums;

namespace Models.SO.Core
{
    [Serializable]
    public struct WallConfigsModel
    {
        public WallTypeEnum WallType;
        public float BaseValue;
        public float AdditionalValue;
        public float CurrentValue;
        public int LevelCount;
        public int BaseCost;
        public int AdditionalCost;
        public int CurrentCost;
        public CurrencyTypeEnum CurrencyType;
    }
}