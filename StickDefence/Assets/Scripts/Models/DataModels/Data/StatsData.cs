using System;
using System.Collections.Generic;
using Enums;
using UnityEngine.Serialization;

namespace Models.DataModels.Data
{
    [Serializable]
    public struct StatsData
    {
        public int CoinsCount;
        public int GemsCount;
        public int StorageIncomeCount;
        public int ChestRewardsCount;
        public int CoinSmeltingLevel;
        public int StorageLevel;
        public long LastDataTimeVisit;    
        public long NextDataLuckySpin;
        public WeeklyDailyData WeeklyDailyData;
    }
    
    [Serializable]
    public struct WeeklyDailyData
    {
        public int LastCollectedDay;
        public long NextDataTimerVisit;
        public bool CanCollectCurrent;
    }
}