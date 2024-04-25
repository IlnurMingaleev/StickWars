using System;
using Enums;

namespace Models.SO.Iaps
{
    [Serializable]
    public struct RewardConfig
    {
        public RewardIconTypeEnum RewardIconType;
        public int CoinReward;
        public int GemReward;
        public int AttackSpeedReward;
        public int GainCoinsReward;
        public int AutoMergeReward;
    }
}