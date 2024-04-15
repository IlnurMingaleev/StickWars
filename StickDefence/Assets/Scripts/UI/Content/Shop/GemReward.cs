using System.Linq;
using TonkoGames.Analytics;
using TonkoGames.Controllers.Core;
using Enums;
using Models.DataModels;
using Models.IAP;
using TMPro;
using Tools.Extensions;
using Tools.GameTools;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.Shop
{
    public class GemReward : UIBehaviour
    {
        [SerializeField] private UIButton _showReward;
        [SerializeField] private TMP_Text _labelCount;
        [SerializeField] private ParticleSystem _chestParticle;

        [Inject] private readonly IAPService _iapService;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ConfigManager _configManager;

        private int _rewardCount = 0;
        
        protected override void Awake()
        {
            base.Awake();
            _rewardCount =
                _configManager.IapSO.IapRewardData.FirstOrDefault(model => model.IapType == IapTypeEnum.GemReward)
                    .Value;
            _labelCount.text = SetScoreExt.ConvertIntToStringValue(_rewardCount, 1);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _iapService.CanShowReward.TakeUntilDisable(this).Subscribe(value =>
            {
                if (value)
                {
                    _chestParticle.Play();
                }
                else
                {
                    _chestParticle.Stop();
                }
                _showReward.IsInteractable = value;
            });
            
            _showReward.OnClick.AsObservable().TakeUntilDisable(this).Subscribe(_ =>
            {
                GameAnalytics.Instance.PushEvent(StringsHelper.Analytics.click_reward, StringsHelper.Analytics.shop_gems);
                _iapService.ShowRewardedBreak(RewardBreak);
            });
        }
        private void RewardBreak(bool value)
        {
            if (value)
            {
                _dataCentralService.StatsDataModel.AddGemsCount(_rewardCount);
                _dataCentralService.SaveFull();
            }
        }
    }
}