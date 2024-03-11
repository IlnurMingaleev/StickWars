using I2.Loc;
using PerfomanceIndex;

namespace Enums
{
    public enum SkillTypesEnum
    {
        None,
        Spikes,
        ShockWave,
        Lightning,
    }
    public static class SkillTypesEnumExtensions
    {
        public static string ToTranslationTerm(this SkillTypesEnum type)
        {
            return SkillType.SkillTypeNames[type];
        }
        
        public static string ToTranslatedName(this SkillTypesEnum type)
        {
            return LocalizationManager.GetTranslation(type.ToTranslationTerm());
        }
        
        public static string ToTranslatedDescription(this SkillTypesEnum type)
        {
            return LocalizationManager.GetTranslation(SkillType.SkillTypeDescriptions[type]);
        }
    }
}