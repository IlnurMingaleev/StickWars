using Enums;
using TonkoGames.Controllers.Core;
using Models.DataModels;
using UniRx;

namespace Models.Player.PumpingFragments
{
    public class SkillsPumping
    {
         private ReactiveDictionary<SkillTypesEnum, PumpingSkillData> _skills =
            new ReactiveDictionary<SkillTypesEnum, PumpingSkillData>();

        public IReadOnlyReactiveDictionary<SkillTypesEnum, PumpingSkillData> Skills => _skills;

        private readonly ConfigManager _configManager;
        private readonly IDataCentralService _dataCentralService;
        
        public SkillsPumping(ConfigManager configManager, IDataCentralService dataCentralService)
        {
            _configManager = configManager;
            _dataCentralService = dataCentralService;
        }
        
        public void Init()
        {
            foreach (var skillType in _configManager.PumpingConfigSo.Skills.Keys)
            {
                _skills.Add(skillType, CreateSkillData(skillType));
            }
        }

        public void UpgradeSkill(SkillTypesEnum skillType)
        {
            if (!_skills[skillType].IsMaxLevel)
            {
                var currentSkillData = _dataCentralService.PumpingDataModel.GetPlayerSkillData(skillType);
                
                currentSkillData.SkillLevel++;
                _dataCentralService.PumpingDataModel.UpdatePlayerSkillData(currentSkillData);
                _skills[skillType] = CreateSkillData(skillType);
                _dataCentralService.SaveFull();
            }
        }
        
        private PumpingSkillData CreateSkillData(SkillTypesEnum skillType)
        {
            var configData = _configManager.PumpingConfigSo.Skills[skillType];
            var currentLevel = _dataCentralService.PumpingDataModel.GetPlayerSkillData(skillType).SkillLevel;

            var skillData = new PumpingSkillData()
            {
                SkillType = skillType,
                Damage = configData.BaseDamage + configData.AdditionalDamage * currentLevel,
                Cost = configData.BaseCost + configData.AdditionalCost * currentLevel,
                CurrentLevel = currentLevel,
                IsMaxLevel = currentLevel == configData.LevelCount - 1,
                CurrencyType = configData.CurrencyType
            };

            return skillData;
        }
    }
}