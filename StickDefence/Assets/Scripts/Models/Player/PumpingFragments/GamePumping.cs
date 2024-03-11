using Enums;
using TonkoGames.Controllers.Core;
using Models.DataModels;
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
            var configData = _configManager.PumpingConfigSo.BasePerks[perkType];
            var perkData = _perks[perkType];
            
            if (!perkData.IsMaxLevel)
            {
                perkData.CurrentLevel++;
                perkData.IsMaxLevel = perkData.CurrentLevel == configData.LevelCount - 1;
                
                _perks[perkType] = UpdatePerkData(perkData);
            }
        }
        
        private PumpingPerkData CreatePerkData(PerkTypesEnum perkType)
        {
            var configData = _configManager.PumpingConfigSo.GamePerks[perkType];

            var perkData = new PumpingPerkData()
            {
                PerkType = perkType,
                Value = _basePerks[perkType].Value + configData.BaseValue + configData.AdditionalValue,
                Cost = configData.BaseCost + configData.AdditionalCost,
                CurrentLevel = 0,
                IsMaxLevel = 0 == configData.LevelCount - 1,
                CurrencyType = configData.CurrencyType
            };

            return perkData;
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