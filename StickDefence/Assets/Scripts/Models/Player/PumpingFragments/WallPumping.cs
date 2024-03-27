using Enums;
using Models.DataModels;
using TonkoGames.Controllers.Core;
using UniRx;

namespace Models.Player.PumpingFragments
{
    public class WallPumping
    {
        private ReactiveDictionary<WallTypeEnum, PumpingWallData> _wallData =
            new ReactiveDictionary<WallTypeEnum, PumpingWallData>();
        private readonly ConfigManager _configManager;
        private readonly IDataCentralService _dataCentralService;

        public IReadOnlyReactiveDictionary<WallTypeEnum, PumpingWallData> WallData =
            new ReactiveDictionary<WallTypeEnum, PumpingWallData>();

        public WallPumping(ConfigManager configManager, IDataCentralService dataCentralService)
        {
            _configManager = configManager;
            _dataCentralService = dataCentralService;
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

            var wallData = new PumpingWallData()
            {
                WallType = wallType,
                Value = configData.BaseValue + configData.AdditionalValue,
                Cost = configData.BaseCost + configData.AdditionalCost,
                CurrentLevel = 0,
                IsMaxLevel = 0 == configData.LevelCount - 1,
                CurrencyType = configData.CurrencyType
            };

            return wallData;
        }

        public void UpgradeWall(WallTypeEnum wallType)
        {
            var configData = _configManager.PumpingConfigSo.WallConfigs[wallType]
                ;
            var perkData = _wallData[wallType];
            
            if (!perkData.IsMaxLevel)
            {
                perkData.CurrentLevel++;
                perkData.IsMaxLevel = perkData.CurrentLevel == configData.LevelCount - 1;
                
                _wallData[wallType] = UpdateWallData(perkData);
            }
        }
        public PumpingWallData UpdateWallData(PumpingWallData pumpingWallData)
        {
            var configData = _configManager.PumpingConfigSo.WallConfigs[pumpingWallData.WallType];
            
            pumpingWallData.CurrencyType = configData.CurrencyType;
            pumpingWallData.Value = configData.BaseValue + configData.AdditionalValue * pumpingWallData.CurrentLevel;
            pumpingWallData.Cost = configData.BaseCost + configData.AdditionalCost * pumpingWallData.CurrentLevel;
            pumpingWallData.IsMaxLevel = pumpingWallData.CurrentLevel == configData.LevelCount - 1;
            
            return pumpingWallData;
        }
    }
}