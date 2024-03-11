using System;
using System.Collections.Generic;
using Enums;
using UnityEngine.Serialization;

namespace Models.DataModels.Data
{
    [Serializable]
    public struct MapStagesData
    {
         public List<MapStageData> MapStageBlockDatas;
    }

    [Serializable]
    public struct MapStageData
    {
        public bool IsAnimationOpened;
        public bool IsCompleted;
        public MapStagesEnum MapStageType;
    }
}