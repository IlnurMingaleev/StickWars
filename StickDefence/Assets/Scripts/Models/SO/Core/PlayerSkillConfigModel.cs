using System;
using Enums;

namespace Models.SO.Core
{
    [Serializable]
    public struct PlayerSkillConfigModel
    {
        public SkillTypesEnum SkillType;
        public float BaseDamage;
        public float AdditionalDamage;
        public int LevelCount;
        public int BaseCost;
        public int AdditionalCost;
        public CurrencyTypeEnum CurrencyType;
    }
}