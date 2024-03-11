using System;
using System.Collections.Generic;
using Enums;

namespace Models.DataModels.Data
{
    [Serializable]
    public struct PlayerPumpingData
    {
        public List<PerkData> PlayerPerksData;
        public List<SkillData> PlayerSkillsData;
        public List<SkillCellData> SkillCellDatas;
    }
    
    [Serializable]
    public struct PerkData
    {
        public PerkTypesEnum PerkType;
        public int PerkLevel;
    }
    
    [Serializable]
    public struct SkillData
    {
        public SkillTypesEnum SkillType;
        public int SkillLevel;
    }

    [Serializable]
    public struct SkillCellData
    {
        public SkillTypesEnum SkillType;
        public bool IsOpen;
    }
}