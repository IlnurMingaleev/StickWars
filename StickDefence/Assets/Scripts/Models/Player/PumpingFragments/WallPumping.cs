using System;
using Enums;
using Models.DataModels;
using TonkoGames.Controllers.Core;
using UniRx;

namespace Models.Player.PumpingFragments
{
    public interface IWallPumpingEvents
    {
        void InitEvents(Action<int> wallUpgradeEvent,Action<int> wallHealthUpgradeEvent);
    }

    public class WallPumping : IWallPumpingEvents
    {
        private ReactiveDictionary<WallTypeEnum, PumpingWallData> _wallData =
            new ReactiveDictionary<WallTypeEnum, PumpingWallData>();
        private readonly ConfigManager _configManager;
        private readonly IDataCentralService _dataCentralService;

        public Action<int> WallCostUpgradeEvent;
        public Action<int> WallHealthUpgradeEvent;
        public IReadOnlyReactiveDictionary<WallTypeEnum, PumpingWallData> WallData => _wallData;

        public WallPumping(ConfigManager configManager, IDataCentralService dataCentralService)
        {
            _configManager = configManager;
            _dataCentralService = dataCentralService;
        }

        public void InitEvents(Action<int> wallUpgradeEvent,Action<int> wallHealthUpgradeEvent)
        {
            WallCostUpgradeEvent = wallUpgradeEvent;
            WallHealthUpgradeEvent = wallHealthUpgradeEvent;
        }

        public void Init()
        {
            _wallData.Clear();
            foreach (var wallType in _configManager.PumpingConfigSo.WallConfigs.Keys)
            {
                _wallData.Add(wallType, CreateWallData(wallType));
            }
        }

        public PumpingWallData CreateWallData(WallTypeEnum wallType)
        {
            var configData = _configManager.PumpingConfigSo.WallConfigs[wallType];
            var level = _dataCentralService.PumpingDataModel.WallLevelReactive.Value.WallLevel;
            var wallData = new PumpingWallData()
            {
                WallType = wallType,
                Value = configData.BaseValue +level* configData.AdditionalValue,
                Cost = configData.BaseCost + level*configData.AdditionalCost,
                HealthValue = configData.BaseHealthValue +level* configData.AdditionalHealthValue,
                CurrentLevel = level,
                IsMaxLevel = level == configData.LevelCount - 1,
                CurrencyType = configData.CurrencyType
            };
            
            WallCostUpgradeEvent?.Invoke(wallData.Cost);
            WallHealthUpgradeEvent?.Invoke(wallData.HealthValue);
            return wallData;
        }

        public void UpgradeWall(WallTypeEnum wallType)
        {
            var configData = _configManager.PumpingConfigSo.WallConfigs[wallType];
            var wallData = _wallData[wallType];
            _dataCentralService.PumpingDataModel.UpgradeWallLevel();
            _dataCentralService.SaveFull();
            var level = _dataCentralService.PumpingDataModel.WallLevelReactive.Value.WallLevel;
            if (!wallData.IsMaxLevel)
            {
                wallData.CurrentLevel = level;
                wallData.IsMaxLevel = wallData.CurrentLevel == configData.LevelCount - 1;
                
                _wallData[wallType] = UpdateWallData(wallData);
            }
        }
        public PumpingWallData UpdateWallData(PumpingWallData pumpingWallData)
        {
            var configData = _configManager.PumpingConfigSo.WallConfigs[pumpingWallData.WallType];
            
            pumpingWallData.CurrencyType = configData.CurrencyType;
            pumpingWallData.Value = configData.BaseValue + configData.AdditionalValue * pumpingWallData.CurrentLevel;
            pumpingWallData.Cost = configData.BaseCost + configData.AdditionalCost * pumpingWallData.CurrentLevel;
            pumpingWallData.HealthValue = configData.BaseHealthValue + configData.AdditionalHealthValue * pumpingWallData.CurrentLevel;
            pumpingWallData.IsMaxLevel = pumpingWallData.CurrentLevel == configData.LevelCount - 1;
           
            WallCostUpgradeEvent?.Invoke(pumpingWallData.Cost);
            WallHealthUpgradeEvent?.Invoke(pumpingWallData.HealthValue);
            return pumpingWallData;
        }
    }
}