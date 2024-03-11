using Enums;
using TonkoGames.Controllers.Core;
using Models.DataModels;
using UniRx;

namespace Models.Player.PumpingFragments
{
    public class BasePumping
    {
        private ReactiveDictionary<PerkTypesEnum, PumpingPerkData> _perks =
            new ReactiveDictionary<PerkTypesEnum, PumpingPerkData>();

        public IReadOnlyReactiveDictionary<PerkTypesEnum, PumpingPerkData> Perks => _perks;

        private readonly ConfigManager _configManager;
        private readonly IDataCentralService _dataCentralService;
        
        public BasePumping(ConfigManager configManager, IDataCentralService dataCentralService)
        {
            _configManager = configManager;
            _dataCentralService = dataCentralService;
        }
        
        public void Init()
        {
            foreach (var perkType in _configManager.PumpingConfigSo.BasePerks.Keys)
            {
                _perks.Add(perkType, CreatePerkData(perkType));
            }
        }

        public void UpgradePerk(PerkTypesEnum perkType)
        {
            var perk = _perks[perkType];

            var currentPerkData = _dataCentralService.PumpingDataModel.GetPlayerPerkData(perkType);
            
            if (!perk.IsMaxLevel)
            {
                currentPerkData.PerkLevel++;
                _dataCentralService.PumpingDataModel.UpdatePlayerPerkData(currentPerkData);
                _perks[perkType] = CreatePerkData(perkType);
                _dataCentralService.SaveFull();
            }
        }
        
        private PumpingPerkData CreatePerkData(PerkTypesEnum perkType)
        {
            var configData = _configManager.PumpingConfigSo.BasePerks[perkType];
            var currentLevel = _dataCentralService.PumpingDataModel.GetPlayerPerkData(perkType).PerkLevel;

            var perkData = new PumpingPerkData()
            {
                PerkType = perkType,
                Value = configData.BaseValue + configData.AdditionalValue * currentLevel,
                Cost = configData.BaseCost + configData.AdditionalCost * currentLevel,
                CurrentLevel = currentLevel,
                IsMaxLevel = currentLevel == configData.LevelCount - 1,
                CurrencyType = configData.CurrencyType
            };

            return perkData;
        }
    }
}