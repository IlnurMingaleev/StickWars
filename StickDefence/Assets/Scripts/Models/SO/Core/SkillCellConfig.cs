using System;
using Enums;

namespace Models.SO.Core
{
    [Serializable]
    public struct SkillCellConfig
    {
        public int Cost;
        public CurrencyTypeEnum CurrencyType;
    }
}