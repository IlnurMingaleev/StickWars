using System.Collections.Generic;
using Enums;
using Models.SO.Core;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace SO.Core
{
    [CreateAssetMenu(fileName = "MapStageSO", menuName = "MyAssets/Config/MapStageSO", order = 5)]
    public class MapStageSO : ScriptableObject
    {
        [SerializeField] private List<MapStageRewardConfig> _mapStageRewardModels;
        [SerializeField] private List<MapStageConfig> _mapBuilder;
        [SerializeField] private List<GroupUnitsConfig> _unitGroups;
        [SerializeField] private int _waveUnitsQty;
        
        private Dictionary<MapStagesEnum, MapStageRewardConfig> _dictionaryMapStageRewardModels =
            new Dictionary<MapStagesEnum, MapStageRewardConfig>();
        
        private Dictionary<MapStagesEnum, MapStageConfig> _dictionaryMapStage =
            new Dictionary<MapStagesEnum, MapStageConfig>();

        private ReactiveProperty<int> _waveUnitsCount = new ReactiveProperty<int>();
        public IReadOnlyDictionary<MapStagesEnum, MapStageRewardConfig> MapStageRewardModels  => _dictionaryMapStageRewardModels;
        public IReadOnlyDictionary<MapStagesEnum, MapStageConfig> MapStages  => _dictionaryMapStage;
        public IReadOnlyList<GroupUnitsConfig> UnitGroups => _unitGroups;
        public IReadOnlyReactiveProperty<int> WaveUnitsCount => _waveUnitsCount;
        public void Init()
        {
            foreach (var mapStage in _mapStageRewardModels)
                _dictionaryMapStageRewardModels.Add(mapStage.StageType, mapStage);
            
            foreach (var mapBuilder in _mapBuilder)
                _dictionaryMapStage.Add(mapBuilder.MapStage, mapBuilder);
            _waveUnitsCount.Value = _waveUnitsQty;
        }
        
#if UNITY_EDITOR

        public void _CONFIG_ONLY_MapStageSO(List<MapStageRewardConfig> mapStageRewardModels,
            List<MapStageConfig> mapBuilder, List<GroupUnitsConfig> groups)
        {
            _mapStageRewardModels = mapStageRewardModels;
            _mapBuilder = mapBuilder;
            _unitGroups = groups;
        }
#endif
    }
}