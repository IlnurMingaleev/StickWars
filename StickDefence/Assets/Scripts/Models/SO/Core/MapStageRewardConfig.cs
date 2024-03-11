using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Models.SO.Core
{
    [Serializable]
    public struct MapStageRewardConfig
    {
        public MapStagesEnum StageType;
        public int Coin;
        public int Gem;
    }
}