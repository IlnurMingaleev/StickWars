using TonkoGames.Controllers.Core;
using Enums;
using I2.Loc;
using Models.SO.Iaps;
using TMPro;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Content.Spin
{
    public class SpinSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _count;

        [Inject] private readonly ConfigManager _configManager;
        
        public RewardConfig Config { get; private set; }
        
        public void Init(RewardConfig rewardIconType)
        {
            Config = rewardIconType;
            _icon.sprite = _configManager.SpritesSo.RewardConfigIconModels[Config.RewardIconType].Image;
            SetCountLabel();
        }
        
        private void SetCountLabel()
        {
            switch (Config.RewardIconType)
            {
                case RewardIconTypeEnum.Gold:
                    _count.text = SetScoreExt.ConvertIntToStringValue(Config.CoinReward, 1).ToString();
                    break;
                case RewardIconTypeEnum.Gem:
                    _count.text = SetScoreExt.ConvertIntToStringValue(Config.GemReward, 1).ToString();
                    break;
                case RewardIconTypeEnum.DefaultChest:
                case RewardIconTypeEnum.VipChest:
                    _count.text = 1.ToString();
                    break;
            }
        }
    }
}