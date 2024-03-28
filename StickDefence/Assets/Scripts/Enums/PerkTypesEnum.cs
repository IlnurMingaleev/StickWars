using I2.Loc;
using PerfomanceIndex;

namespace Enums
{
    public enum PerkTypesEnum
    {
        DecreasePrice,
        IncreaseProfit,
        RecruitsDamage,
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