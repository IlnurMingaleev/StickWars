using System;
using Enums;

namespace Models.SO.Core
{
    [Serializable]
    public struct UnitStatsConfig
    {
        public UnitTypeEnum UnitType;
        public int Health;
        public float Armor;
        public int Damage;
        public int Reloading;
        public int Experience;
        public int Coins;
    }
}