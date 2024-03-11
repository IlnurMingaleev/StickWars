using System.Collections.Generic;
using Enums;
using I2.Loc;

namespace PerfomanceIndex
{
    public class PerkType
    {
        public static List<PerkTypesEnum> UnitTypes { get; } = new List<PerkTypesEnum>()
        {
            PerkTypesEnum.Damage,
            PerkTypesEnum.AttackSpeed,
            PerkTypesEnum.AttackRange,
            PerkTypesEnum.CriticalChance,
            PerkTypesEnum.CriticalMultiplier,
            PerkTypesEnum.Health,
            PerkTypesEnum.RegenHealth,
            PerkTypesEnum.Defense,
            PerkTypesEnum.KillSilverBonus,
            PerkTypesEnum.DailyGoldBonus,
        };
        
        public static Dictionary<PerkTypesEnum, string> PerkTypeNames { get; } = new Dictionary<PerkTypesEnum, string>()
        {
            { PerkTypesEnum.Damage, ScriptTerms.Names_Perks.Damage },
            { PerkTypesEnum.AttackSpeed, ScriptTerms.Names_Perks.AttackSpeed },
            { PerkTypesEnum.AttackRange, ScriptTerms.Names_Perks.AttackRange },
            { PerkTypesEnum.CriticalChance, ScriptTerms.Names_Perks.CriticalChance },
            { PerkTypesEnum.CriticalMultiplier, ScriptTerms.Names_Perks.CriticalMultiplier },
            { PerkTypesEnum.Health, ScriptTerms.Names_Perks.Health },
            { PerkTypesEnum.RegenHealth, ScriptTerms.Names_Perks.RegenHealth },
            { PerkTypesEnum.Defense, ScriptTerms.Names_Perks.Defense },
            { PerkTypesEnum.KillSilverBonus, ScriptTerms.Names_Perks.KillSilverBonus },
            { PerkTypesEnum.DailyGoldBonus, ScriptTerms.Names_Perks.DailyGoldBonus },
        };
    }
}