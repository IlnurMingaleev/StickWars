using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using Models.Controllers;
using Models.DataModels;
using Models.DataModels.Data;
using Models.Player;
using Models.SO.Iaps;
using UI.Content.DailyBonus;
using UI.Content.Rewards;
using UI.UIManager;
using UnityEngine;
using VContainer;

namespace UI.Windows
{
    public class WeeklyDailyBonusWindow : Window
    {
        [SerializeField] private List<WeeklyDailyBlock> _weeklyDailyBlocks;

        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly IPlayer _player;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < _weeklyDailyBlocks.Count; i++)
            {
                RewardConfig rewardConfig = _configManager.IapSO.WeeklyRewardModel[i];

                _weeklyDailyBlocks[i].Init(i, this, rewardConfig);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
            _manager.AddCurrentWindow(this);
            SetButtons();
#if UNITY_WEBGL
            _manager.GetWindow<TopPanelWindow>().ChowSettingsButton(false);
#endif
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
#if UNITY_WEBGL
            _manager.GetWindow<TopPanelWindow>().ChowSettingsButton(true);
#endif
        }

        private void SetButtons()
        {
            WeeklyDailyData weeklyDailyData = _dataCentralService.StatsDataModel.WeeklyDailyData.Value;

            for (int i = 0; i < _weeklyDailyBlocks.Count; i++)
            {
                bool collected = i < weeklyDailyData.LastCollectedDay;
                bool selectable = weeklyDailyData.CanCollectCurrent && i ==  weeklyDailyData.LastCollectedDay;// LastCollectedDay не индекс поэтому инста +1  от индекса

                _weeklyDailyBlocks[i].InitDay(selectable, collected);
            }
        }

        public void OnCollect(RewardConfig rewardConfig)
        {
            _manager.Show<RewardCollectWindow>(WindowPriority.AboveTopPanel).Collect(new RewardContains()
            {
                Coin = rewardConfig.CoinReward,
                Gem = rewardConfig.GemReward
            }).Forget();
            
            _player.DailyModel.DailyCollect(rewardConfig);
            SetButtons();
        }
    }
}