using System;
using Enums;

namespace Models.Player.PumpingFragments
{
    [Serializable]
    public struct PumpingPerkData
    {
        public PerkTypesEnum PerkType;
        public float Value;
        public int Cost;
        public int CurrentLevel;
        public bool IsMaxLevel;
        public CurrencyTypeEnum CurrencyType;
    }
}