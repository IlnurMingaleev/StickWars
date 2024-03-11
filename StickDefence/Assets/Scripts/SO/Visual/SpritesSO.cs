using System.Collections.Generic;
using Enums;
using Models.SO.Visual;
using UnityEngine;

namespace SO.Visual
{
    [CreateAssetMenu(fileName = "SpritesSO", menuName = "MyAssets/Config/SpritesSO", order = 2)]
    public class SpritesSO : ScriptableObject
    {
        [SerializeField] private List<RewardConfigIconModel> _rewardConfigIconModel;
        [SerializeField] private List<UISkillsIconsConfig> _uiSkillsIconsConfig;
        
        private Dictionary<SkillTypesEnum, Sprite> _uiSkillsIcons =
            new Dictionary<SkillTypesEnum, Sprite>();
        private Dictionary<RewardIconTypeEnum, RewardConfigIconModel> _dictionaryRewardConfigIconModel =
            new Dictionary<RewardIconTypeEnum, RewardConfigIconModel>();
        public IReadOnlyDictionary<SkillTypesEnum, Sprite> UISkillsIcons  => _uiSkillsIcons;
        public IReadOnlyDictionary<RewardIconTypeEnum, RewardConfigIconModel> RewardConfigIconModels =>
            _dictionaryRewardConfigIconModel;
        
        public void Init()
        {
            foreach (var config in _uiSkillsIconsConfig)
                _uiSkillsIcons.Add(config.SkillType, config.Value);
            

            foreach (var rewardConfig in _rewardConfigIconModel)
                _dictionaryRewardConfigIconModel.Add(rewardConfig.RewardIconType, rewardConfig);
        }
    }
}