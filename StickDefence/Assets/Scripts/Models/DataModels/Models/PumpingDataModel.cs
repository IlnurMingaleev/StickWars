using System.Linq;
using Enums;
using Models.DataModels.Data;
using TonkoGames.Controllers.Core;
using UniRx;

namespace Models.DataModels.Models
{
     public interface IPumpingDataModel
    {
        #region Fields
        IReadOnlyReactiveDictionary<PerkTypesEnum, PerkData> PerksReactive { get; }
        IReadOnlyReactiveDictionary<SkillTypesEnum, SkillData> SkillsReactive { get; }
        IReadOnlyReactiveCollection<SkillCellData> SkillCellsReactive { get; }
        IReadOnlyReactiveProperty<WallData> WallLevelReactive { get; }
        public IReadOnlyReactiveProperty<LevelData> LevelReactive { get; }
        IReadOnlyReactiveProperty<PlayerUnitTypeEnum> MaxStickmanLevel { get; }
        IReadOnlyReactiveProperty<MapStagesEnum> StageLoadType { get; }

        #endregion
        
        #region Setters

        void UpgradeWallLevel();
        void SetStageIndex(int mapStageIndex);
        void UpdatePlayerPerkData(PerkData perkData);
        PerkData GetPlayerPerkData(PerkTypesEnum unitTypeEnum);
        void UpdatePlayerSkillData(SkillData skillData);
        SkillData GetPlayerSkillData(SkillTypesEnum unitTypeEnum);

        void UpdateSkillCellData(int index, SkillCellData skillCellData);
        SkillCellData GetSkillCellData(int index);
        void UpgradeMaxStickmanLevel();
        public void IncreaseExperience(int gainedExp, ConfigManager configManager);
        public void CalculateRequiredExp(ConfigManager configManager);

        #endregion
    } 
    public class PumpingDataModel : IPumpingDataModel
    {
        #region Fields

        private ReactiveProperty<LevelData> _currentLevelData = new ReactiveProperty<LevelData>();
        private ReactiveProperty<PlayerUnitTypeEnum> _maxStickmanLevel = new ReactiveProperty<PlayerUnitTypeEnum>();

        private ReactiveDictionary<PerkTypesEnum, PerkData> _playerPerks =
            new ReactiveDictionary<PerkTypesEnum, PerkData>();

        private ReactiveDictionary<SkillTypesEnum, SkillData> _playerSkills =
            new ReactiveDictionary<SkillTypesEnum, SkillData>();
        public IReadOnlyReactiveProperty<PlayerUnitTypeEnum> MaxStickmanLevel => _maxStickmanLevel;

        private ReactiveCollection<SkillCellData> _skillCellsReactive = new ReactiveCollection<SkillCellData>();
        
        private ReactiveProperty<WallData> _wallData = new ReactiveProperty<WallData>();
        private ReactiveProperty<MapStagesEnum> _stageLoadType = new ReactiveProperty<MapStagesEnum>();
        public IReadOnlyReactiveProperty<MapStagesEnum> StageLoadType => _stageLoadType;

        public IReadOnlyReactiveDictionary<PerkTypesEnum, PerkData> PerksReactive => _playerPerks;
        public IReadOnlyReactiveDictionary<SkillTypesEnum, SkillData> SkillsReactive => _playerSkills;
        public IReadOnlyReactiveCollection<SkillCellData> SkillCellsReactive => _skillCellsReactive;
        public IReadOnlyReactiveProperty<WallData> WallLevelReactive => _wallData;
       
        public IReadOnlyReactiveProperty<LevelData> LevelReactive => _currentLevelData;
        #endregion

        #region Setters

        public void UpgradeWallLevel() => _wallData.Value = new WallData()
        {
            WallTypeEnum = WallTypeEnum.Basic,
            WallLevel = _wallData.Value.WallLevel + 1,
        };

        public void SetStageIndex(int mapStageIndex) => _stageLoadType.Value = (mapStageIndex <= ((int) MapStagesEnum.Stage1_20))?(MapStagesEnum)mapStageIndex: MapStagesEnum.Stage1_20;
        public void UpgradeMaxStickmanLevel() => _maxStickmanLevel.Value += 1;

        public void UpdatePlayerPerkData(PerkData perkData) => _playerPerks[perkData.PerkType] = perkData;

        public PerkData GetPlayerPerkData(PerkTypesEnum unitTypeEnum) => _playerPerks.TryGetValue(unitTypeEnum, out var data) ? data : CreateNewPlayerPerkData(unitTypeEnum);

        private PerkData CreateNewPlayerPerkData(PerkTypesEnum unitTypeEnum)
        {
            PerkData perkData = CreateNewPerk(unitTypeEnum);
            _playerPerks.Add(unitTypeEnum, perkData);

            return perkData;
        }
        
        public void UpdatePlayerSkillData(SkillData skillData) => _playerSkills[skillData.SkillType] = skillData;

        public SkillData GetPlayerSkillData(SkillTypesEnum skillTypesEnum) => _playerSkills.TryGetValue(skillTypesEnum, out var data) ? data : CreateNewPlayerSkillData(skillTypesEnum);

        private SkillData CreateNewPlayerSkillData(SkillTypesEnum skillTypesEnum)
        {
            var skillData = new SkillData
            {
                SkillType = skillTypesEnum,
            };
            _playerSkills.Add(skillTypesEnum, skillData);

            return skillData;
        }

        public void UpdateSkillCellData(int index, SkillCellData skillCellData) => _skillCellsReactive[index] = skillCellData;

        public SkillCellData GetSkillCellData(int index) {
            if (_skillCellsReactive.Count <= index)
            {
                _skillCellsReactive.Add(new SkillCellData());
            }
            return _skillCellsReactive[index];
        }

        #region Experience

        public void IncreaseExperience(int gainedExp,ConfigManager configManager)
        {
            int exp = _currentLevelData.Value.CurrentExp;
            int requireExp = _currentLevelData.Value.RequiredExp;
            exp += gainedExp;
            if (exp >= requireExp)
            {
                while (exp >= requireExp)
                {
                    exp -= requireExp;
                    LevelUp(configManager);
                    requireExp = _currentLevelData.Value.RequiredExp;
                }
            }

            _currentLevelData.Value = new LevelData()
            {
                Level = _currentLevelData.Value.Level,
                CurrentExp = exp,
                RequiredExp = requireExp,
            };
        }

        public void LevelUp(ConfigManager configManager)
        {
            _currentLevelData.Value =
                new LevelData()
                {
                    Level = _currentLevelData.Value.Level + 1,
                    CurrentExp = _currentLevelData.Value.CurrentExp,
                    RequiredExp = _currentLevelData.Value.RequiredExp,
                };
            CalculateRequiredExp(configManager);
        }

        public void CalculateRequiredExp(ConfigManager configManager)
        {
            _currentLevelData.Value = new LevelData()
            {
                Level = _currentLevelData.Value.Level,
                CurrentExp = _currentLevelData.Value.CurrentExp,
                RequiredExp = configManager.UnitsStatsSo.GetRequiredExp(_currentLevelData.Value.Level),
            };
        }

        #endregion
      
        

        #endregion

        #region Storage
        public PlayerPumpingData GetData()
        {
            PlayerPumpingData statsData = new PlayerPumpingData
            {
                PlayerPerksData = _playerPerks.Values.ToList(),
                PlayerSkillsData = _playerSkills.Values.ToList(),
                SkillCellDatas = _skillCellsReactive.ToList(),
                MaxStickmanLevel = _maxStickmanLevel.Value,
                LevelData =  _currentLevelData.Value,
                MapStagesType = _stageLoadType.Value,
                WallData = _wallData.Value,
            };
            return statsData;
        }

        public void SetPlayerPumpingData(PlayerPumpingData playerPumpingData)
        {
            _playerPerks.Clear();
            _playerSkills.Clear();
            _skillCellsReactive.Clear();
            foreach (var playerCharacterData in playerPumpingData.PlayerPerksData)
            {
                _playerPerks.Add(playerCharacterData.PerkType, playerCharacterData);
            }
            
            foreach (var playerSkillsData in playerPumpingData.PlayerSkillsData)
            {
                _playerSkills.Add(playerSkillsData.SkillType, playerSkillsData);
            }
            

            _maxStickmanLevel.Value = playerPumpingData.MaxStickmanLevel;
            _currentLevelData.Value = playerPumpingData.LevelData;
            _stageLoadType.Value = playerPumpingData.MapStagesType;
            _wallData.Value = playerPumpingData.WallData;
        }
        
        
        public void SetAndInitEmptyPlayerPumpingData(PlayerPumpingData playerPumpingData)
        {
            playerPumpingData.PlayerPerksData = new ()
            {
                new PerkData()
                {
                    PerkType = PerkTypesEnum.DecreasePrice,
                    PerkLevel = 1,
                },
               
                new PerkData()
                {
                    PerkType = PerkTypesEnum.IncreaseProfit,
                    PerkLevel = 1,
                },
                new PerkData()
                {
                    PerkType = PerkTypesEnum.RecruitsDamage,
                    PerkLevel = 1,
                },
            };
            playerPumpingData.PlayerSkillsData = new ()
            {
                new SkillData()
                {
                    SkillType = SkillTypesEnum.Grenade,
                    SkillLevel = 1,
                },
                new SkillData()
                {
                    SkillType = SkillTypesEnum.Rocket,
                    SkillLevel = 1,
                },
                new SkillData()
                {
                    SkillType = SkillTypesEnum.Gas,
                    SkillLevel = 1,
                }
            };
            playerPumpingData.MaxStickmanLevel = PlayerUnitTypeEnum.One;
            playerPumpingData.MapStagesType = MapStagesEnum.Stage1_1;
            playerPumpingData.LevelData = new LevelData
            {
                CurrentExp = 0,
                Level = 1,
                RequiredExp = 10,
            };
            playerPumpingData.WallData = new WallData()
            {
                WallTypeEnum = WallTypeEnum.Basic,
                WallLevel = 0,
            };
            SetPlayerPumpingData(playerPumpingData);
        }

        private PerkData CreateNewPerk(PerkTypesEnum perkTypesEnum)
        {
            PerkData perkData = new PerkData()
            {
                PerkType = perkTypesEnum
            };

            switch (perkTypesEnum)
            {
                case PerkTypesEnum.DecreasePrice:
                case PerkTypesEnum.IncreaseProfit:
                case PerkTypesEnum.RecruitsDamage:
                    perkData.PerkLevel = 0;
                    break;
                default:
                    perkData.PerkLevel = 0;
                    break;
            }

            return perkData;
        }
        #endregion  
    }
}