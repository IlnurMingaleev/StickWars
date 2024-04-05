using System;
using Enums;

namespace Models.Player.PumpingFragments
{
    [Serializable]
    public struct PumpingSkillData
    {
        public SkillTypesEnum SkillType;
        public float Damage;
        public int Cost;
        public int CurrentLevel;
        public bool IsMaxLevel;
        public CurrencyTypeEnum CurrencyType;
    }
}