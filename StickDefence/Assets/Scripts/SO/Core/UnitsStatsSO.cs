using System.Collections.Generic;
using Enums;
using Models.SO.Core;
using Tools.Configs;
using UnityEngine;

namespace SO.Core
{
    [CreateAssetMenu(fileName = "UnitsStatsSO", menuName = "MyAssets/Config/UnitsStatsSO", order = 4)]
    public class UnitsStatsSO : ScriptableObject
    {
        [SerializeField] private List<UnitStatsConfig> _enemyUnitConfig;
        [SerializeField] private List<UnitRewardConfig> _unitRewardConfigs;
        [SerializeField] private List<StickmanStatsConfig> stickmanUnitsStatsConfigs;
       // [SerializeField] private List<PlayerLevelConfig> _playerLevelConfigsList;

        [Header("Animation Curve")] public AnimationCurve animationCurve;
        public int MaxLevel;
        public int MaxRequiredExp;


        private Dictionary<UnitTypeEnum, UnitRewardConfig> _dictionaryUnitRewardConfigs =
            new Dictionary<UnitTypeEnum, UnitRewardConfig>();

        private Dictionary<UnitTypeEnum, UnitStatsConfig> _dictionaryEnemyConfigs =
            new Dictionary<UnitTypeEnum, UnitStatsConfig>();

        private Dictionary<int, PlayerLevelConfig> _dictionaryLevelConfigs = new Dictionary<int, PlayerLevelConfig>();
        public List<StickmanStatsConfig> StickmanUnitsStatsConfigs => stickmanUnitsStatsConfigs;

        private  Dictionary<PlayerUnitTypeEnum,StickmanStatsConfig> _dictionaryStickmanConfigs =
            new Dictionary<PlayerUnitTypeEnum, StickmanStatsConfig>();

        public Dictionary<UnitTypeEnum, UnitStatsConfig> EnemyUnitConfigs =>
            _dictionaryEnemyConfigs;

        public Dictionary<UnitTypeEnum, UnitRewardConfig> UnitRewardConfigs =>
            _dictionaryUnitRewardConfigs;

        public IReadOnlyDictionary<PlayerUnitTypeEnum,StickmanStatsConfig> DictionaryStickmanConfigs =>
            _dictionaryStickmanConfigs;

        public IReadOnlyDictionary<int, PlayerLevelConfig> PlayerLevelConfigs => _dictionaryLevelConfigs;

        public void Init()
        {
            _dictionaryStickmanConfigs.Clear();
            _dictionaryLevelConfigs.Clear();
            foreach (var unitConfig in stickmanUnitsStatsConfigs)
            {
                _dictionaryStickmanConfigs.Add(unitConfig.UnitType, unitConfig);
            }

            foreach (var enemyConfigModel in _enemyUnitConfig)
                _dictionaryEnemyConfigs.Add(enemyConfigModel.UnitType, enemyConfigModel);

            foreach (var unitRewardConfigs in _unitRewardConfigs)
                _dictionaryUnitRewardConfigs.Add(unitRewardConfigs.UnitType, unitRewardConfigs);
            //foreach (var levelConfig in _playerLevelConfigsList)
            //    _dictionaryLevelConfigs.Add(levelConfig.Level, levelConfig);

        }

        public int GetRequiredExp(int level)
        {
            int requiredExp = Mathf.RoundToInt(animationCurve.Evaluate(Mathf.InverseLerp(0, MaxLevel, level)) * MaxRequiredExp);
            return requiredExp;
        }
#if UNITY_EDITOR

        public void _CONFIG_ONLY_EnemyUnitsStatsSO(List<UnitStatsConfig> enemyUnitConfig, List<UnitRewardConfig> unitRewards,List<StickmanStatsConfig> stickmanStatsConfigs)
        {
            _enemyUnitConfig = enemyUnitConfig;
            _unitRewardConfigs = unitRewards;
            stickmanUnitsStatsConfigs = stickmanStatsConfigs;
        }
        
#endif
    }
}