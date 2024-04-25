using System;
using Cysharp.Threading.Tasks;
using Enums;
using TonkoGames.Controllers.Core;
using Helpers.Time;
using Models.Battle.Boosters;
using Models.DataModels;
using Models.DataModels.Data;
using Models.SO.Core;
using Models.SO.Iaps;
using Models.Timers;
using Tools.Extensions;
using UniRx;
using UnityEngine;

namespace Models.Controllers
{
    public interface IDailyModel
    {
        void DailyCollect(RewardConfig rewardConfig);
        IReadOnlyReactiveProperty<bool> CanSpin { get; }
        
        IReadOnlyReactiveProperty<int> SpinTimer { get; }
        void UpdateSpin(RewardConfig rewardConfig);
        void RewardSpinShown();
        void InitBoosterManager(BoosterManager boosterManager);
    }
    public class DailyModel : IDailyModel
    {
        private const int ForgeTickRate = 5; //Тик каждые 5 секунд
        
        private readonly IDataCentralService _dataCentralService;
        private readonly ConfigManager _configManager;
        private readonly ITimerService _timerService;
        private ReactiveProperty<bool> _canSpin = new ReactiveProperty<bool>();
        private ReactiveProperty<int> _spinTimer = new ReactiveProperty<int>();
        private BoosterManager _boosterManager;
        private ITimerModel _timerModelSpin = null;
        private const int SecondsCooldownSpin = 20; //TODO Fix spin cooldown time
        public IReadOnlyReactiveProperty<int> SpinTimer => _spinTimer;

        public IReadOnlyReactiveProperty<bool> CanSpin => _canSpin; 

        public DailyModel(IDataCentralService dataCentralService, ConfigManager configManager,
            ITimerService timerService)
        {
            _dataCentralService = dataCentralService;
            _configManager = configManager;
            _timerService = timerService;
           
        }

        public void InitBoosterManager(BoosterManager boosterManager)
        {
            _boosterManager = boosterManager;
        }

        public void CheckDailyModel()
        {
            CheckCoinFarmerIncomeLastTimeDelta();
            CheckWeeklyDailyData();
            CheckCanSpin();
        }
        
        private void CheckCoinFarmerIncomeLastTimeDelta()
        {
            CoinFarmerConfigModel coinSmeltingConfigModel =
                _configManager.PumpingConfigSo.CoinFarmerConfigModel[
                    _dataCentralService.StatsDataModel.CoinSmeltingLevel.Value];
            
            CoinFarmerConfigModel storageConfigModel =
                _configManager.PumpingConfigSo.CoinFarmerConfigModel[
                    _dataCentralService.StatsDataModel.StorageLevel.Value];
            
            if (_dataCentralService.StatsDataModel.StorageIncomeCount.Value < storageConfigModel.StorageCapacity)
            {
                long deltaSec = TimeHelpers.DataTimeToTimeStamp(DateTime.Now) - _dataCentralService.StatsDataModel.LastDataTimeVisit.Value;
                
                int tmpCoinsIncome = _dataCentralService.StatsDataModel.StorageIncomeCount.Value + (coinSmeltingConfigModel.CoinSmeltingGoldTick * (int)((int) deltaSec / ForgeTickRate));
                if (tmpCoinsIncome > storageConfigModel.StorageCapacity)
                {
                    tmpCoinsIncome = storageConfigModel.StorageCapacity;
                }
                        
                _dataCentralService.StatsDataModel.SetStorageIncomeCount(tmpCoinsIncome);
            }
        }

        private void CheckWeeklyDailyData()
        {
            WeeklyDailyData weeklyDailyData = _dataCentralService.StatsDataModel.WeeklyDailyData.Value;
            
            DateTime nextDateTime = TimeHelpers.TimeStampToDataTime(weeklyDailyData.NextDataTimerVisit);
            DateTime currentDateTime = DateTime.Now.ClearHours();
           
            if (currentDateTime >= nextDateTime)
                weeklyDailyData.CanCollectCurrent = true;
            _dataCentralService.StatsDataModel.UpdateWeeklyDailyData(weeklyDailyData);
        }

        public void DailyCollect(RewardConfig rewardConfig)
        {
            WeeklyDailyData weeklyDailyData = _dataCentralService.StatsDataModel.WeeklyDailyData.Value;

            weeklyDailyData.CanCollectCurrent = false;
            weeklyDailyData.LastCollectedDay++;
            if (weeklyDailyData.LastCollectedDay >= 7)
            {
                weeklyDailyData.LastCollectedDay = 0;
            }
            weeklyDailyData.NextDataTimerVisit = TimeHelpers.DataTimeToTimeStamp(DateTime.Now.ClearHours().AddDays(1));
            _dataCentralService.StatsDataModel.UpdateWeeklyDailyData(weeklyDailyData);

            _dataCentralService.StatsDataModel.AddCoinsCount(rewardConfig.CoinReward);
            _dataCentralService.StatsDataModel.AddGemsCount(rewardConfig.GemReward);

            _dataCentralService.SaveFull();
        }

        private void CheckCanSpin()
        {
            long currentUnix = TimeHelpers.DataTimeToTimeStamp(DateTime.Now);
            if (_timerModelSpin != null)
            {
                _timerModelSpin.StopTick();
                _timerModelSpin = null;
            }
            
            if (_dataCentralService.StatsDataModel.NextDataLuckySpin.Value > currentUnix)
            {
                _canSpin.Value = false;
                var delta = _dataCentralService.StatsDataModel.NextDataLuckySpin.Value - currentUnix;
                _spinTimer.Value = (int)delta;
                _timerModelSpin = _timerService.AddDefaultTimer(delta, _ => _spinTimer.Value = (int)_, () => _canSpin.Value = true);
            }
            else
            {
                _canSpin.Value = true;
            }
        }

        public void UpdateSpin(RewardConfig rewardConfig)
        {
            _dataCentralService.StatsDataModel.AddCoinsCount(rewardConfig.CoinReward);
            _dataCentralService.StatsDataModel.AddGemsCount(rewardConfig.GemReward);
            if (_boosterManager)
            {
                if (rewardConfig.AttackSpeedReward > 0)
                {
                    _boosterManager.ApplyBooster(BoosterTypeEnum.AttackSpeed);
                }

                if (rewardConfig.AutoMergeReward > 0)
                {
                    _boosterManager.ApplyBooster(BoosterTypeEnum.AutoMerge);
                }

                if (rewardConfig.GainCoinsReward > 0)
                {
                    _boosterManager.ApplyBooster(BoosterTypeEnum.GainCoins);
                }
            }

            var nowTime = TimeHelpers.DataTimeToTimeStamp(DateTime.Now);
            nowTime += SecondsCooldownSpin;
            _dataCentralService.StatsDataModel.SetNextDataLuckySpin(nowTime);
            
            _dataCentralService.SaveFull();
            CheckCanSpin();
        }

        public void RewardSpinShown()
        {
            if (_timerModelSpin != null)
            {
                _timerModelSpin.StopTick();
                _timerModelSpin = null;
            }
        }
    }
}