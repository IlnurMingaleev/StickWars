using System;
using System.Collections.Generic;
using Enums;
using Models.DataModels.Models;
using Models.Merge;
using UnityEngine.Serialization;

namespace Models.DataModels.Data
{
    [Serializable]
    public struct MapStagesData
    {
         public List<MapStageData> MapStageBlockDatas;
         public List<SlotItemData> SlotItemDatas;
         public MapStagesEnum LastMapStage;
    }

    [Serializable]
    public struct MapStageData
    {
        public bool IsAnimationOpened;
        public bool IsCompleted;
        public MapStagesEnum MapStageType;
        
    }
    [Serializable]
    public struct SlotItemData
    {
        public SlotIdTypeEnum SlotIdTypeEnum;
        public PlayerUnitTypeEnum PlayerUnitType;
    }
}