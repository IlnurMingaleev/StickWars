using System.Collections.Generic;
using Enums;
using I2.Loc;

namespace PerfomanceIndex
{
    public class SkillType
    {
        public static List<SkillTypesEnum> SkillTypes { get; } = new List<SkillTypesEnum>()
        {
            SkillTypesEnum.Rocket,
            SkillTypesEnum.Grenade,
            SkillTypesEnum.Gas,

        };
        
        public static Dictionary<SkillTypesEnum, string> SkillTypeNames { get; } = new Dictionary<SkillTypesEnum, string>()
        {
            { SkillTypesEnum.Rocket, ScriptTerms.Names_Skills.Spikes },
            { SkillTypesEnum.Grenade, ScriptTerms.Names_Skills.ShockWave },
            { SkillTypesEnum.Gas, ScriptTerms.Names_Skills.Lightning },
        };
        
        public static Dictionary<SkillTypesEnum, string> SkillTypeDescriptions { get; } = new Dictionary<SkillTypesEnum, string>()
        {
            { SkillTypesEnum.Rocket, ScriptTerms.Windows_UpgradeBaseWindow.SpikesDescription },
            { SkillTypesEnum.Grenade, ScriptTerms.Windows_UpgradeBaseWindow.ShockWaveDescription },
            { SkillTypesEnum.Gas, ScriptTerms.Windows_UpgradeBaseWindow.LightningDescription },
        };
    }
}