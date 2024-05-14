using I2.Loc;
using PerfomanceIndex;

namespace Enums
{
    public enum PerkTypesEnum
    {
        DecreasePrice = 0,
        IncreaseProfit = 1,
        RecruitsDamage = 2,
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