using System.Collections.Generic;
using Enums;
using I2.Loc;

namespace PerfomanceIndex
{
    public class SkillType
    {
        public static List<SkillTypesEnum> SkillTypes { get; } = new List<SkillTypesEnum>()
        {
            SkillTypesEnum.Spikes,
            SkillTypesEnum.ShockWave,
            SkillTypesEnum.Lightning,

        };
        
        public static Dictionary<SkillTypesEnum, string> SkillTypeNames { get; } = new Dictionary<SkillTypesEnum, string>()
        {
            { SkillTypesEnum.Spikes, ScriptTerms.Names_Skills.Spikes },
            { SkillTypesEnum.ShockWave, ScriptTerms.Names_Skills.ShockWave },
            { SkillTypesEnum.Lightning, ScriptTerms.Names_Skills.Lightning },
        };
        
        public static Dictionary<SkillTypesEnum, string> SkillTypeDescriptions { get; } = new Dictionary<SkillTypesEnum, string>()
        {
            { SkillTypesEnum.Spikes, ScriptTerms.Windows_UpgradeBaseWindow.SpikesDescription },
            { SkillTypesEnum.ShockWave, ScriptTerms.Windows_UpgradeBaseWindow.ShockWaveDescription },
            { SkillTypesEnum.Lightning, ScriptTerms.Windows_UpgradeBaseWindow.LightningDescription },
        };
    }
}