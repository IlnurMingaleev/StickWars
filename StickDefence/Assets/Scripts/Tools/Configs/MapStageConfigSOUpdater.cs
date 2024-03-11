using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Models.Move;
using Models.SO.Core;
using SO.Core;
using UnityEditor;
using UnityEngine;

namespace Tools.Configs
{
    [CreateAssetMenu(fileName = "MapStageConfigSOUpdater", menuName = "MyAssets/EditorOnly/MapStageConfigSOUpdater", order = 2)]
    public class MapStageConfigSOUpdater // : DataContainerBase
    {
        [SerializeField] private MapStageSO _mapStageSo;
        
        [ContextMenu("Update config")]
        private void CompressionConfigLevels()
        {
            CompressionMapStage();
            EditorUtility.SetDirty(_mapStageSo);
            Debug.Log("Update MapStageConfigSOUpdater");
        }
        
       // [PageName("MapStageReward")] 
        [HideInInspector] public List<MapStageRewardConfig> MapStageReward;
        
       // [PageName("MapBuilder")] 
        [HideInInspector] public List<EditorMapStageLine> MapBuilder;
        
       // [PageName("GroupUnits")] 
        [HideInInspector] public List<EditorGroupUnitLine> GroupUnits;
        
        [Serializable]
        public struct EditorMapStageLine
        {
            public MapStagesEnum MapStage;
            public int Day;
            public int GroupUnitsIndex;
            public float Delay;
            public bool IsBossDay;
        }
        
        [Serializable] 
        public struct EditorGroupUnitLine
        {
            public int Index;
            public UnitTypeEnum UnitType;
            public int Count;
            public float Delay;
            public UnitMovementTypeEnum MovementType;
        }
        
        private void CompressionMapStage()
        {
            #region MapBuilder

            var mapBuilder =  MapBuilder
                .GroupBy(stage => stage.MapStage)
                .Select(mapStageGroup => new MapStageConfig
                {
                    MapStage = mapStageGroup.Key,
                    Days = mapStageGroup
                        .GroupBy(stage => stage.Day)
                        .Select(dayGroup => new MapStageDayConfig
                        {
                            IsBossDay = dayGroup.First().IsBossDay,
                            Groups = dayGroup
                                .Select(day => new MapStageDayGroupConfig
                                {
                                    GroupUnitsIndex = day.GroupUnitsIndex,
                                    Delay = day.Delay
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            #endregion

            #region GroupsUnits
            
            var groupsUnit = GroupUnits
                .GroupBy(unit => unit.Index)
                .Select(indexGroup => new GroupUnitsConfig
                {
                    Units = indexGroup
                        .Select(unit => new GroupUnitConfig
                        {
                            UnitType = unit.UnitType,
                            Count = unit.Count,
                            Delay = unit.Delay,
                            MovementType = unit.MovementType
                        })
                        .ToList()
                })
                .ToList();

            #endregion
            
            _mapStageSo._CONFIG_ONLY_MapStageSO(MapStageReward, mapBuilder, groupsUnit);
        }
        
    }
}