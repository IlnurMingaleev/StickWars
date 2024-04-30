using System;
using Enums;

namespace Models.SO.Core
{
    [Serializable]
    public struct StickmanStatsConfig
    {
        public PlayerUnitTypeEnum UnitType;
        public int Level;
        public int Damage;
        public int Reloading;
        public int Price;
        public float AttackSpeed;
        
    }
 
}