using System.Collections.Generic;
using Enums;
using I2.Loc;
using PerfomanceIndex;

namespace Models.IAP.InApps
{
    public enum RealCurrencyEnum
    {
        YAN,
        OK,
        VOTE,
        RUB,
        GP,
        DOLLAR
    }
    
    public static class RealCurrencyEnumExtensions
    {
        public static string ToTranslationTerm(this RealCurrencyEnum type)
        {
            return RealCurrencyNames[type];
        }
        
        public static string ToTranslatedName(this RealCurrencyEnum type)
        {
            return LocalizationManager.GetTranslation(type.ToTranslationTerm());
        }
        
        private static Dictionary<RealCurrencyEnum, string> RealCurrencyNames { get; } = new Dictionary<RealCurrencyEnum, string>()
        {
            { RealCurrencyEnum.YAN, ScriptTerms.Inaps.YAN },
            { RealCurrencyEnum.OK, ScriptTerms.Inaps.OK },
            { RealCurrencyEnum.VOTE, ScriptTerms.Inaps.VOTE },
            { RealCurrencyEnum.RUB, ScriptTerms.Inaps.RUB },
            { RealCurrencyEnum.GP, ScriptTerms.Inaps.GP },
            { RealCurrencyEnum.DOLLAR, ScriptTerms.Inaps.DOLLAR },
        };
    }
}