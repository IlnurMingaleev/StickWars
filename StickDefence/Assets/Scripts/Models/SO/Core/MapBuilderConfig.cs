using System;
using System.Collections.Generic;
using Enums;

namespace Models.SO.Core
{

    [Serializable]
    public struct MapStageConfig
    {
        public MapStagesEnum MapStage;
        public List<MapStageDayConfig> Days;
    }
    
    [Serializable]
    public struct MapStageDayConfig
    {
        public bool IsBossDay;
        public List<MapStageDayGroupConfig> Groups;
    }
    
    [Serializable]
    public struct MapStageDayGroupConfig
    {
        public int GroupUnitsIndex;
        public float Delay;
    }
}