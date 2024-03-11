using System;
using System.Collections.Generic;
using Models.IAP.InApps;
using Models.SO.Iaps;
using UnityEngine;

namespace SO.Iaps
{
    [CreateAssetMenu(fileName = "IAPSO", menuName = "MyAssets/Config/IAPSO")]
    public class IAPSO : ScriptableObject
    {
        [SerializeField] private List<IapRewardData> _iapRewardData;
        [SerializeField] private List<RewardConfig> _weeklyReward;
        [SerializeField] private List<RewardConfig> _luckySpin;
        [SerializeField] private List<InAppRewardConfig> _inAppRewardConfig;

        public IReadOnlyList<IapRewardData> IapRewardData => _iapRewardData;
        public IReadOnlyList<RewardConfig> WeeklyRewardModel => _weeklyReward;
        public IReadOnlyList<RewardConfig> LuckySpin => _luckySpin;
        
        private Dictionary<PaymentProductEnum, InAppRewardConfig> _dictionaryAppReward =
            new Dictionary<PaymentProductEnum, InAppRewardConfig>();
        
        public IReadOnlyDictionary<PaymentProductEnum, InAppRewardConfig> AppRewards  => _dictionaryAppReward;
        
        public void Init()
        {
            foreach (var appReward in _inAppRewardConfig)
                _dictionaryAppReward.Add(appReward.PaymentProduct, appReward);
        }
        
#if UNITY_EDITOR

        public void _CONFIG_ONLY_IAPSO(List<IapRewardData> iapRewardData, List<RewardConfig> weeklyReward, List<RewardConfig> luckySpin, List<InAppRewardConfig> inAppRewardConfig)
        {
            _iapRewardData = iapRewardData;
            _weeklyReward = weeklyReward;
            _luckySpin = luckySpin;
            _inAppRewardConfig = inAppRewardConfig;
        }
#endif
    }
}