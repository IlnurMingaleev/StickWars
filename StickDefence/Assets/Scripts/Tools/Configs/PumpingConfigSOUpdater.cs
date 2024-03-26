using System.Collections.Generic;
using Models.SO.Core;
using NorskaLib.GoogleSheetsDatabase;
using SO.Core;
using UnityEditor;
using UnityEngine;

namespace Tools.Configs
{
    [CreateAssetMenu(fileName = "PumpingConfigSOUpdater", menuName = "MyAssets/EditorOnly/PumpingConfigSOUpdater", order = 1)]
    public class PumpingConfigSOUpdater : DataContainerBase
    {
        [SerializeField] private PumpingConfigSO _pumpingConfig;
        
        [ContextMenu("Update config")]
        private void CompressionConfigLevels()
        {
            CompressionPumping();
            
            EditorUtility.SetDirty(_pumpingConfig);
            Debug.Log("Update Config");
        }
        
        [PageName("CoinFarmer")] 
        [HideInInspector] public List<CoinFarmerConfigModel> CoinFarmer;

        [PageName("BasePlayerPerks")] 
        [HideInInspector] public List<PlayerPerkConfigModel> BasePlayerPerks;
        
        [PageName("GamePlayerPerks")] 
        [HideInInspector] public List<PlayerPerkConfigModel> GamePlayerPerks;
        
        [PageName("PlayerSkills")] 
        [HideInInspector] public List<PlayerSkillConfigModel> PlayerSkills;
        
        [PageName("SkillCells")] 
        [HideInInspector] public List<SkillCellConfig> SkillCells;
        
        [PageName("WallConfig")] 
        [HideInInspector] public List<WallConfigsModel> WallConfigsModels;
        
        private void CompressionPumping()
        {
            _pumpingConfig._CONFIG_ONLY_InitConfig(CoinFarmer, BasePlayerPerks, GamePlayerPerks, PlayerSkills, SkillCells, WallConfigsModels);
        }
        
    }
}