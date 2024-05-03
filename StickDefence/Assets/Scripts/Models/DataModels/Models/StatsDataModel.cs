using System;
using Helpers.Time;
using Models.DataModels.Data;
using Tools.Extensions;
using UniRx;

namespace Models.DataModels.Models
{
 public interface IStatsDataModel
    {
       #region Fields
        IReadOnlyReactiveProperty<int> ChestRewardsCount { get; } 

        IReadOnlyReactiveProperty<int> StorageIncomeCount { get; }

        IReadOnlyReactiveProperty<int> CoinsCount { get; }

        IReadOnlyReactiveProperty<int> GemsCount { get; }
        IReadOnlyReactiveProperty<int> CoinSmeltingLevel { get; }
        IReadOnlyReactiveProperty<int> StorageLevel { get; }
        IReadOnlyReactiveProperty<long> LastDataTimeVisit { get; }
        IReadOnlyReactiveProperty<long> NextDataLuckySpin { get; }
        
        IReadOnlyReactiveProperty<WeeklyDailyData> WeeklyDailyData { get; }

        #endregion
        
        #region Setters
        void SetCoinsCount(int value);
        void AddCoinsCount(int value);
        void MinusCoinsCount(int value);
        void SetGemsCount(int value);
        void AddGemsCount(int value);
        void MinusGemsCount(int value);
        void SetStorageIncomeCount(int value);

        void AddCoinsIncomeCount(int value);

        void SetChestRewardsCount(int value);
        
        void SetCoinSmeltingLevel(int value);
        
        void SetStorageLevel(int value);
        void SetLastDataTimeVisit();
        void SetNextDataLuckySpin(long time);
        void UpdateWeeklyDailyData(WeeklyDailyData weeklyDailyData);

        #endregion
    }
    
    public class StatsDataModel : IStatsDataModel
    {
        #region Fields

        private ReactiveProperty<int> _chestRewardsCount = new ReactiveProperty<int>();
        private ReactiveProperty<int> _coinsCount = new ReactiveProperty<int>();
        private ReactiveProperty<int> _gemsCount = new ReactiveProperty<int>();
        private ReactiveProperty<int> _storageIncomeCount = new ReactiveProperty<int>();
        private ReactiveProperty<int> _сoinSmeltingLevel = new ReactiveProperty<int>();
        private ReactiveProperty<int> _storageLevel = new ReactiveProperty<int>();
        private ReactiveProperty<long> _lastDataTimeVisit = new ReactiveProperty<long>();
        private ReactiveProperty<long> _nextDataLuckySpin = new ReactiveProperty<long>();
        private ReactiveProperty<WeeklyDailyData> _weeklyDailyData = new ReactiveProperty<WeeklyDailyData>();
        
        public IReadOnlyReactiveProperty<int> ChestRewardsCount => _chestRewardsCount;
        public IReadOnlyReactiveProperty<int> CoinsCount => _coinsCount;
        public IReadOnlyReactiveProperty<int> GemsCount => _gemsCount;
        public IReadOnlyReactiveProperty<int> StorageIncomeCount => _storageIncomeCount;
        public IReadOnlyReactiveProperty<int> CoinSmeltingLevel => _сoinSmeltingLevel;
        public IReadOnlyReactiveProperty<int> StorageLevel => _storageLevel;
        public IReadOnlyReactiveProperty<long> LastDataTimeVisit => _lastDataTimeVisit;
        public IReadOnlyReactiveProperty<long> NextDataLuckySpin => _nextDataLuckySpin;
        public IReadOnlyReactiveProperty<WeeklyDailyData> WeeklyDailyData => _weeklyDailyData;

        #endregion

        #region Setters
        public void SetCoinsCount(int value) => _coinsCount.Value = value;
        public void AddCoinsCount(int value) => _coinsCount.Value += value;
        public void MinusCoinsCount(int value) => _coinsCount.Value -= value;
        public void SetGemsCount(int value) => _gemsCount.Value = value;
        public void AddGemsCount(int value) => _gemsCount.Value += value;
        public void MinusGemsCount(int value) => _gemsCount.Value -= value;
        public void SetStorageIncomeCount(int value) => _storageIncomeCount.Value = value;
        public void SetChestRewardsCount(int value) => _chestRewardsCount.Value = value;
        public void AddCoinsIncomeCount(int value) => _storageIncomeCount.Value += value;
        public void SetCoinSmeltingLevel(int value) => _сoinSmeltingLevel.Value = value;
        public void SetStorageLevel(int value) => _storageLevel.Value = value;
        public void SetLastDataTimeVisit() => _lastDataTimeVisit.Value = TimeHelpers.DataTimeToTimeStamp(DateTime.Now);
        public void SetNextDataLuckySpin(long time) => _nextDataLuckySpin.Value = time;
        public void UpdateWeeklyDailyData(WeeklyDailyData weeklyDailyData) => _weeklyDailyData.Value = weeklyDailyData;
        
        #endregion

        #region Storage
        public StatsData GetStatsData()
        {
            _lastDataTimeVisit.Value = TimeHelpers.DataTimeToTimeStamp(DateTime.Now);
            
            StatsData statsData = new StatsData
            {
                CoinsCount = _coinsCount.Value,
                GemsCount = _gemsCount.Value,
                ChestRewardsCount = _chestRewardsCount.Value,
                StorageIncomeCount = _storageIncomeCount.Value,
                CoinSmeltingLevel = _сoinSmeltingLevel.Value,
                StorageLevel = _storageLevel.Value,
                LastDataTimeVisit = _lastDataTimeVisit.Value,
                NextDataLuckySpin = _nextDataLuckySpin.Value,
                WeeklyDailyData = _weeklyDailyData.Value,
            };
            return statsData;
        }

        public void SetStatsData(StatsData statsData)
        {
            _coinsCount.Value = statsData.CoinsCount;
            _gemsCount.Value = statsData.GemsCount;
            _chestRewardsCount.Value = statsData.ChestRewardsCount;
            _storageIncomeCount.Value = statsData.StorageIncomeCount;
            _сoinSmeltingLevel.Value = statsData.CoinSmeltingLevel;
            _storageLevel.Value = statsData.StorageLevel;
            _lastDataTimeVisit.Value = statsData.LastDataTimeVisit;
            _nextDataLuckySpin.Value = statsData.NextDataLuckySpin;
            
            if (statsData.WeeklyDailyData.NextDataTimerVisit == 0)
                statsData.WeeklyDailyData.NextDataTimerVisit = TimeHelpers.DataTimeToTimeStamp(DateTime.Now.ClearHours());
            
            _weeklyDailyData.Value = statsData.WeeklyDailyData;
        }
        
        public void SetAndInitEmptyStatsData(StatsData statsData)
        {
            statsData.CoinsCount = 200;
            statsData.GemsCount = 0;
            statsData.StorageLevel = 1;
            statsData.CoinSmeltingLevel = 1;
            statsData.LastDataTimeVisit = TimeHelpers.DataTimeToTimeStamp(DateTime.Now);
            SetStatsData(statsData);
        }
        #endregion  
    }
}