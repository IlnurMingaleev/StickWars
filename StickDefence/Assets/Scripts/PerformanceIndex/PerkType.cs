using System.Collections.Generic;
using Enums;
using I2.Loc;

namespace PerfomanceIndex
{
    public class PerkType
    {
        public static List<PerkTypesEnum> UnitTypes { get; } = new List<PerkTypesEnum>()
        {
           PerkTypesEnum.DecreasePrice,
           PerkTypesEnum.IncreaseProfit,
           PerkTypesEnum.RecruitsDamage,
        };
        
        public static Dictionary<PerkTypesEnum, string> PerkTypeNames { get; } = new Dictionary<PerkTypesEnum, string>()
        {
            { PerkTypesEnum.DecreasePrice, ScriptTerms.Names_Perks.DecreasePrice },
            { PerkTypesEnum.IncreaseProfit, ScriptTerms.Names_Perks.IncreaseProfit },
            { PerkTypesEnum.RecruitsDamage, ScriptTerms.Names_Perks.RecruitsDamage },
        };
    }
}