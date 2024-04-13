using Enums;
using TonkoGames.Controllers.Core;
using Models.DataModels;
using Models.DataModels.Data;
using UniRx;
using UnityEngine;

namespace Models.Player.PumpingFragments
{
    public class GamePumping
    {
        private IReadOnlyReactiveDictionary<PerkTypesEnum, PumpingPerkData> _basePerks;
        
        private ReactiveDictionary<PerkTypesEnum, PumpingPerkData> _perks =
            new ReactiveDictionary<PerkTypesEnum, PumpingPerkData>();

        public IReadOnlyReactiveDictionary<PerkTypesEnum, PumpingPerkData> Perks => _perks;

        private readonly ConfigManager _configManager;
        private readonly IDataCentralService _dataCentralService;
        
        public GamePumping(IReadOnlyReactiveDictionary<PerkTypesEnum, PumpingPerkData> basePerks, ConfigManager configManager, IDataCentralService dataCentralService)
        {
            _basePerks = basePerks;
            _configManager = configManager;
            _dataCentralService = dataCentralService;
        }

        public void BattleLoad()
        {
            _perks.Clear();
            
            foreach (var perkType in _configManager.PumpingConfigSo.GamePerks.Keys)
            {
                _perks.Add(perkType, CreatePerkData(perkType));
            }
        }
        
        public void UpgradePerk(PerkTypesEnum perkType)
        {
            var configData = _configManager.PumpingConfigSo.GamePerks[perkType];
            var pumpingPerkData = _perks[perkType];
            var perkData = _dataCentralService.PumpingDataModel.PerksReactive[perkType];
            if (!pumpingPerkData.IsMaxLevel)
            {
                pumpingPerkData.CurrentLevel = perkData.PerkLevel + 1;
                pumpingPerkData.IsMaxLevel = pumpingPerkData.CurrentLevel == configData.LevelCount - 1;
                _perks[perkType] = UpdatePerkData(pumpingPerkData);
                _dataCentralService.PumpingDataModel.UpdatePlayerPerkData(new PerkData()
                {
                    PerkLevel = perkData.PerkLevel + 1,
                    PerkType = perkType
                        
                });
                _dataCentralService.SaveFull();
            }
        }
        
        private PumpingPerkData CreatePerkData(PerkTypesEnum perkType)
        {
            var configData = _configManager.PumpingConfigSo.GamePerks[perkType];
            var perkData = _dataCentralService.PumpingDataModel.PerksReactive[perkType];
            var pumpingPerkData = new PumpingPerkData()
            {
                PerkType = perkType,
                Value = _basePerks[perkType].Value + configData.BaseValue + configData.AdditionalValue * perkData.PerkLevel,
                Cost = configData.BaseCost + configData.AdditionalCost* perkData.PerkLevel,
                CurrentLevel =  perkData.PerkLevel,
                IsMaxLevel = perkData.PerkLevel == configData.LevelCount - 1,
                CurrencyType = configData.CurrencyType
            };

            return pumpingPerkData;
        }

        private PumpingPerkData UpdatePerkData(PumpingPerkData pumpingPerkData)
        {
            var configData = _configManager.PumpingConfigSo.GamePerks[pumpingPerkData.PerkType];
            
            pumpingPerkData.CurrencyType = configData.CurrencyType;
            pumpingPerkData.Value = _basePerks[pumpingPerkData.PerkType].Value + configData.BaseValue + configData.AdditionalValue * pumpingPerkData.CurrentLevel;
            pumpingPerkData.Cost = configData.BaseCost + configData.AdditionalCost * pumpingPerkData.CurrentLevel;
            pumpingPerkData.IsMaxLevel = pumpingPerkData.CurrentLevel == configData.LevelCount - 1;
            
            return pumpingPerkData;
        }
    }
}