using System;
using Enums;
using TonkoGames.Controllers.Core;
using Models.DataModels;
using Models.Player.PumpingFragments;
using UniRx;

namespace Models.Player
{
    public interface IPumping
    {
        public IWallPumpingEvents WallPumpingEvents { get; }
        IReadOnlyReactiveDictionary<PerkTypesEnum, PumpingPerkData> BasePerks { get; }
        IReadOnlyReactiveDictionary<SkillTypesEnum, PumpingSkillData> Skills { get; }
        IReadOnlyReactiveDictionary<PerkTypesEnum, PumpingPerkData> GamePerks { get; }
        IReadOnlyReactiveDictionary<WallTypeEnum, PumpingWallData> WallData { get; }

        void UpgradeBasePerk(PerkTypesEnum perkType);
        void UpgradeGamePerk(PerkTypesEnum perkType);
        void UpgradeSkill(SkillTypesEnum skillType);
        void UpgradeWall(WallTypeEnum wallType);
        void BattleLoad();
    }
    public class Pumping : IPumping
    {
        private BasePumping _basePumping;
        private GamePumping _gamePumping;
        private SkillsPumping _skillsPumping;
        private WallPumping _wallPumping;

        public IWallPumpingEvents WallPumpingEvents=> _wallPumping;
        public Pumping(ConfigManager configManager, IDataCentralService dataCentralService)
        {
            _basePumping = new BasePumping(configManager, dataCentralService);
            _skillsPumping = new SkillsPumping(configManager, dataCentralService);
            _gamePumping = new GamePumping(BasePerks, configManager, dataCentralService);
            _wallPumping = new WallPumping(configManager, dataCentralService);
        }

        public void Init()
        {
            _basePumping.Init();
            _skillsPumping.Init();
            
        }
        
        public void BattleLoad()
        {
            _wallPumping.Init();
            _gamePumping.BattleLoad();
        }

        public IReadOnlyReactiveDictionary<PerkTypesEnum, PumpingPerkData> BasePerks => _basePumping.Perks;

        public IReadOnlyReactiveDictionary<SkillTypesEnum, PumpingSkillData> Skills => _skillsPumping.Skills;

        public IReadOnlyReactiveDictionary<PerkTypesEnum, PumpingPerkData> GamePerks => _gamePumping.Perks;

        public IReadOnlyReactiveDictionary<WallTypeEnum, PumpingWallData> WallData => _wallPumping.WallData;

        public void UpgradeBasePerk(PerkTypesEnum perkType) => _basePumping.UpgradePerk(perkType);

        public void UpgradeGamePerk(PerkTypesEnum perkType) => _gamePumping.UpgradePerk(perkType);

        public void UpgradeSkill(SkillTypesEnum skillType) => _skillsPumping.UpgradeSkill(skillType);

        public void UpgradeWall(WallTypeEnum wallType) => _wallPumping.UpgradeWall(wallType);
    }
}