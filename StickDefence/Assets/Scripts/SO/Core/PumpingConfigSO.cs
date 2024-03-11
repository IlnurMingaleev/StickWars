using System.Collections.Generic;
using Enums;
using Models.SO.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace SO.Core
{
    [CreateAssetMenu(fileName = "PumpingConfigSO", menuName = "MyAssets/Config/PumpingConfigSO", order = 3)]
    public class PumpingConfigSO : ScriptableObject
    {
        [SerializeField] private List<CoinFarmerConfigModel> _coinFarmerConfigModel;
        [SerializeField] private List<PlayerPerkConfigModel> _basePlayerPerks;
        [SerializeField] private List<PlayerPerkConfigModel> _gamePlayerPerks;
        [SerializeField] private List<PlayerSkillConfigModel> _playerSkills;
        [SerializeField] private List<SkillCellConfig> _skillCellConfigs;

        public IReadOnlyList<CoinFarmerConfigModel> CoinFarmerConfigModel => _coinFarmerConfigModel;

        public IReadOnlyList<SkillCellConfig> SkillCells => _skillCellConfigs;

        private Dictionary<PerkTypesEnum, PlayerPerkConfigModel> _basePerks =
            new Dictionary<PerkTypesEnum, PlayerPerkConfigModel>();
        
        private Dictionary<PerkTypesEnum, PlayerPerkConfigModel> _gamePerks =
            new Dictionary<PerkTypesEnum, PlayerPerkConfigModel>();
        
        private Dictionary<SkillTypesEnum, PlayerSkillConfigModel> _skills =
            new Dictionary<SkillTypesEnum, PlayerSkillConfigModel>();
        public IReadOnlyDictionary<PerkTypesEnum, PlayerPerkConfigModel> BasePerks  => _basePerks;
        public IReadOnlyDictionary<PerkTypesEnum, PlayerPerkConfigModel> GamePerks  => _gamePerks;
        public IReadOnlyDictionary<SkillTypesEnum, PlayerSkillConfigModel> Skills  => _skills;
        public void Init()
        {
            foreach (var config in _basePlayerPerks)
                _basePerks.Add(config.PerkType, config);
            
            foreach (var config in _gamePlayerPerks)
                _gamePerks.Add(config.PerkType, config);
            
            foreach (var config in _playerSkills)
                _skills.Add(config.SkillType, config);
        }

#if UNITY_EDITOR
        public void _CONFIG_ONLY_InitConfig(List<CoinFarmerConfigModel> foundryConfigModel, 
            List<PlayerPerkConfigModel> basePlayerPerks, 
            List<PlayerPerkConfigModel> gamePlayerPerks,
            List<PlayerSkillConfigModel> playerSkills,
            List<SkillCellConfig> skillCellConfigs)
        {
            _coinFarmerConfigModel = foundryConfigModel;
            _basePlayerPerks = basePlayerPerks;
            _gamePlayerPerks = gamePlayerPerks;
            _playerSkills = playerSkills;
            _skillCellConfigs = skillCellConfigs;
        }
#endif
    }
}