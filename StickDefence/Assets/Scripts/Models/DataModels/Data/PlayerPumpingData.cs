using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Models.DataModels.Data
{
    [Serializable]
    public struct PlayerPumpingData
    {
        public List<PerkData> PlayerPerksData;
        public List<SkillData> PlayerSkillsData;
        public List<SkillCellData> SkillCellDatas;
        public PlayerUnitTypeEnum MaxStickmanLevel;
        public LevelData LevelData;
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

    [Serializable]
    public struct WallData
    {
        public WallTypeEnum WallTypeEnum;
        public int WallLevel;
    }

    [Serializable]

    public struct LevelData
    {
        public int Level;
        public int CurrentExp;
        public int RequiredExp;
    }
}