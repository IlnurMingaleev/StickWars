using System;
using Enums;
using UI.Content.Rewards;

namespace Models.SO.Core
{
    [Serializable]
    public struct UnitRewardConfig
    {
        public UnitTypeEnum UnitType;
        public RewardType RewardType;
        public int RewardCount;
        public int Experience;
    }
}