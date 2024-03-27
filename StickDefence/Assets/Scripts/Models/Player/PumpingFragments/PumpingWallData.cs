using Enums;

namespace Models.Player.PumpingFragments
{
    public struct PumpingWallData
    { 
        public WallTypeEnum WallType;
        public float Value;
        public int Cost;
        public int CurrentLevel;
        public bool IsMaxLevel;
        public CurrencyTypeEnum CurrencyType;
    }
}