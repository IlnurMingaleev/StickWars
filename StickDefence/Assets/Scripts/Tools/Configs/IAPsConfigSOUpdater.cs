using System.Collections.Generic;
using Models.SO.Iaps;
using NorskaLib.GoogleSheetsDatabase;
using SO.Iaps;
using UnityEditor;
using UnityEngine;

namespace Tools.Configs
{
    [CreateAssetMenu(fileName = "IAPsConfigSOUpdater", menuName = "MyAssets/EditorOnly/IAPsConfigSOUpdater", order = 4)]
    public class IAPsConfigSOUpdater :DataContainerBase
    {
        [SerializeField] private IAPSO _iAPSO;
        
        [ContextMenu("Update config")]
        private void CompressionConfigLevels()
        {
            UpdateIAPSO();
            
            EditorUtility.SetDirty(_iAPSO);
            Debug.Log("Update Config");
        }
        
        [PageName("IAP")] 
        [HideInInspector] public List<IapRewardData> IapRewardData;
        
        [PageName("WeeklyReward")] 
        [HideInInspector] public List<RewardConfig> WeeklyReward;
        
       [PageName("LuckySpin")] 
        [HideInInspector] public List<RewardConfig> LuckySpin;
        
        [PageName("InApp")] 
        [HideInInspector] public List<InAppRewardConfig> InApp;
        private void UpdateIAPSO()
        {
            _iAPSO._CONFIG_ONLY_IAPSO(IapRewardData, WeeklyReward, LuckySpin, InApp);
        }
    }
}