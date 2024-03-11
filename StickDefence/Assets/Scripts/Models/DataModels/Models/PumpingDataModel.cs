using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Models.DataModels.Data;
using UniRx;
using UnityEngine;

namespace Models.DataModels.Models
{
     public interface IPumpingDataModel
    {
        #region Fields
        IReadOnlyReactiveDictionary<PerkTypesEnum, PerkData> PerksReactive { get; }
        IReadOnlyReactiveDictionary<SkillTypesEnum, SkillData> SkillsReactive { get; }
        IReadOnlyReactiveCollection<SkillCellData> SkillCellsReactive { get; }

        #endregion
        
        #region Setters
        void UpdatePlayerPerkData(PerkData perkData);
        PerkData GetPlayerPerkData(PerkTypesEnum unitTypeEnum);
        void UpdatePlayerSkillData(SkillData skillData);
        SkillData GetPlayerSkillData(SkillTypesEnum unitTypeEnum);

        void UpdateSkillCellData(int index, SkillCellData skillCellData);
        SkillCellData GetSkillCellData(int index);

        #endregion
    } 
    public class PumpingDataModel : IPumpingDataModel
    {
        #region Fields

        private ReactiveDictionary<PerkTypesEnum, PerkData> _playerPerks =
            new ReactiveDictionary<PerkTypesEnum, PerkData>();
        
        private ReactiveDictionary<SkillTypesEnum, SkillData> _playerSkills =
            new ReactiveDictionary<SkillTypesEnum, SkillData>();

        private ReactiveCollection<SkillCellData> _skillCellsReactive = new ReactiveCollection<SkillCellData>();

        public IReadOnlyReactiveDictionary<PerkTypesEnum, PerkData> PerksReactive => _playerPerks;
        public IReadOnlyReactiveDictionary<SkillTypesEnum, SkillData> SkillsReactive => _playerSkills;
        public IReadOnlyReactiveCollection<SkillCellData> SkillCellsReactive => _skillCellsReactive;

        #endregion

        #region Setters

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
        
        
        #endregion

        #region Storage
        public PlayerPumpingData GetCharacterPumpingData()
        {
            PlayerPumpingData statsData = new PlayerPumpingData
            {
                PlayerPerksData = _playerPerks.Values.ToList(),
                PlayerSkillsData = _playerSkills.Values.ToList(),
                SkillCellDatas = _skillCellsReactive.ToList()
            };
            return statsData;
        }

        public void SetPlayerPumpingData(PlayerPumpingData playerPumpingData)
        {
            foreach (var playerCharacterData in playerPumpingData.PlayerPerksData)
            {
                _playerPerks.Add(playerCharacterData.PerkType, playerCharacterData);
            }
            
            foreach (var playerSkillsData in playerPumpingData.PlayerSkillsData)
            {
                _playerSkills.Add(playerSkillsData.SkillType, playerSkillsData);
            }

            _skillCellsReactive = playerPumpingData.SkillCellDatas.ToReactiveCollection();
        }
        
        
        public void SetAndInitEmptyPlayerPumpingData(PlayerPumpingData playerPumpingData)
        {
            _playerPerks.Clear();
            playerPumpingData.PlayerPerksData = new ();
            playerPumpingData.PlayerSkillsData = new ();
            playerPumpingData.SkillCellDatas = new ();
            
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
                case PerkTypesEnum.Damage:
                case PerkTypesEnum.AttackSpeed:
                case PerkTypesEnum.Health:
                case PerkTypesEnum.AttackRange:
                    perkData.PerkLevel = 1;
                    break;
                case PerkTypesEnum.CriticalChance:
                case PerkTypesEnum.CriticalMultiplier:
                case PerkTypesEnum.RegenHealth:
                case PerkTypesEnum.Defense:
                case PerkTypesEnum.KillSilverBonus:
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