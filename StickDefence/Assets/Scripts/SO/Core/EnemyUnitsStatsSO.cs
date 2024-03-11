using System.Collections.Generic;
using Enums;
using Models.SO.Core;
using UnityEngine;

namespace SO.Core
{
    [CreateAssetMenu(fileName = "EnemyUnitsStatsSO", menuName = "MyAssets/Config/EnemyUnitsStatsSO", order = 4)]
    public class EnemyUnitsStatsSO : ScriptableObject
    {
        [SerializeField] private List<UnitStatsConfig> _enemyUnitConfig;
        [SerializeField] private List<UnitRewardConfig> _unitRewardConfigs;
        
        private Dictionary<UnitTypeEnum, UnitRewardConfig> _dictionaryUnitRewardConfigs =
            new Dictionary<UnitTypeEnum, UnitRewardConfig>();
        
        private Dictionary<UnitTypeEnum, UnitStatsConfig> _dictionaryEnemyConfigs =
            new Dictionary<UnitTypeEnum, UnitStatsConfig>();
        
        public Dictionary<UnitTypeEnum, UnitStatsConfig> EnemyUnitConfigs =>
            _dictionaryEnemyConfigs;
        
        public Dictionary<UnitTypeEnum, UnitRewardConfig> UnitRewardConfigs =>
            _dictionaryUnitRewardConfigs;
        
        public void Init()
        {
            foreach (var enemyConfigModel in _enemyUnitConfig)
                _dictionaryEnemyConfigs.Add(enemyConfigModel.UnitType, enemyConfigModel);
            
            foreach (var unitRewardConfigs in _unitRewardConfigs)
                _dictionaryUnitRewardConfigs.Add(unitRewardConfigs.UnitType, unitRewardConfigs);
        }
        
#if UNITY_EDITOR

        public void _CONFIG_ONLY_EnemyUnitsStatsSO(List<UnitStatsConfig> enemyUnitConfig, List<UnitRewardConfig> unitRewards)
        {
            _enemyUnitConfig = enemyUnitConfig;
            _unitRewardConfigs = unitRewards;
        }
        
#endif
    }
}