using System.Collections.Generic;
using Enums;
using Models.SO.Core;
using UnityEngine;

namespace SO.Core
{
    [CreateAssetMenu(fileName = "StickmanStatsSO", menuName = "MyAssets/Config/StickmanStatsSO", order = 6)]
    public class StickmanUnitStatsSO : ScriptableObject
    {
        [SerializeField] private List<StickmanStatsConfig> _stickmanUnitsStatsConfigs;

        public IReadOnlyCollection<StickmanStatsConfig> StickmanStatsConfigs => _stickmanUnitsStatsConfigs;

        private Dictionary<PlayerUnitTypeEnum,StickmanStatsConfig> _dictionaryStickmanConfigs =
            new Dictionary<PlayerUnitTypeEnum, StickmanStatsConfig>();

        public IReadOnlyDictionary<PlayerUnitTypeEnum, StickmanStatsConfig> DictionaryStickmanConfigs =>
            _dictionaryStickmanConfigs;

        public void Init()
        {
            _dictionaryStickmanConfigs.Clear();
            foreach (var unitConfig in _stickmanUnitsStatsConfigs)
            {
                _dictionaryStickmanConfigs.Add(unitConfig.UnitType, unitConfig);
            }

        }
        
#if UNITY_EDITOR

        public void _CONFIG_ONLY_StickmanUnitsConfigs(List<StickmanStatsConfig> stickmanStatsConfigs)
        {
            _stickmanUnitsStatsConfigs = stickmanStatsConfigs;
        }
        
#endif
    }
}