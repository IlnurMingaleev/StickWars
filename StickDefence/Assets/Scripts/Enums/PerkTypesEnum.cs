using I2.Loc;
using PerfomanceIndex;

namespace Enums
{
    public enum PerkTypesEnum
    {
        Damage,
        AttackSpeed,
        AttackRange,
        CriticalChance,
        CriticalMultiplier,
        Health,
        RegenHealth,
        Defense,
        KillSilverBonus,
        DailyGoldBonus
    }
    
    public static class PerkTypeEnumExtensions
    {
        public static string ToTranslationTerm(this PerkTypesEnum type)
        {
            return PerkType.PerkTypeNames[type];
        }
        
        public static string ToTranslatedName(this PerkTypesEnum type)
        {
            return LocalizationManager.GetTranslation(type.ToTranslationTerm());
        }
    }
}