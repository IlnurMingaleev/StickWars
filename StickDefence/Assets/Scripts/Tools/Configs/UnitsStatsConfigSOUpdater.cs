using System;
using System.Collections.Generic;
using Enums;
using Models.SO.Core;
using NorskaLib.GoogleSheetsDatabase;
using SO.Core;
using UI.Content.Rewards;
using UnityEditor;
using UnityEngine;

namespace Tools.Configs
{
    [CreateAssetMenu(fileName = "UnitsStatsConfigSOUpdater", menuName = "MyAssets/EditorOnly/UnitsStatsConfigSOUpdater", order = 3)]
    public class UnitsStatsConfigSOUpdater  : DataContainerBase
    {
        [SerializeField] private UnitsStatsSO unitsStatsSo;
        
        [ContextMenu("Update config")]
        private void CompressionConfigLevels()
        {
            UpdateEnemyUnitConfigModel();
            
            EditorUtility.SetDirty(unitsStatsSo);
            Debug.Log("Update Config");
        }

        [PageName("EnemyUnitsStats")] 
        [HideInInspector] public List<EnemyUnitConfig> EnemyUnitConfigModel;
        [PageName("StickManUnitStats")] 
        [HideInInspector] public List<StickmanStatsConfig> StickmanStatsConfigModel;
        
        [Serializable]
        public struct EnemyUnitConfig
        {
            public UnitTypeEnum UnitType;
            public int Health;
            public float Armor;
            public int Damage;
            public float Reloading;
            public RewardType RewardType;
            public int RewardCount;
            public int Experience;
        }
        
      
        private void UpdateEnemyUnitConfigModel()
        {
            List<UnitRewardConfig> unitRewards = new();
            List<UnitStatsConfig> unitConfigs = new();
            List<StickmanStatsConfig> stickmanStatsConfigs = new();
            
            foreach (var enemyUnitConfig in EnemyUnitConfigModel)
            {
                unitRewards.Add(new UnitRewardConfig()
                {
                    UnitType = enemyUnitConfig.UnitType,
                    RewardType = enemyUnitConfig.RewardType,
                    RewardCount = enemyUnitConfig.RewardCount,
                    Experience = enemyUnitConfig.Experience,
                });
                
                unitConfigs.Add(new UnitStatsConfig()
                {
                    UnitType = enemyUnitConfig.UnitType,
                    Health = enemyUnitConfig.Health,
                    Armor = enemyUnitConfig.Armor,
                    Damage = enemyUnitConfig.Damage,
                    Reloading = enemyUnitConfig.Reloading,
                   
                });
            }

            foreach (var stickmanStatsConfig in StickmanStatsConfigModel)
            {
                stickmanStatsConfigs.Add(new StickmanStatsConfig()
                {
                    UnitType = stickmanStatsConfig.UnitType,
                    Level = stickmanStatsConfig.Level, 
                    Damage = stickmanStatsConfig.Damage,
                    AttackSpeed = stickmanStatsConfig.AttackSpeed,
                    Reloading = stickmanStatsConfig.Reloading,
                    Price = stickmanStatsConfig.Price,

                });
            }
            
            unitsStatsSo._CONFIG_ONLY_EnemyUnitsStatsSO(unitConfigs, unitRewards,stickmanStatsConfigs);
        }
    }
}