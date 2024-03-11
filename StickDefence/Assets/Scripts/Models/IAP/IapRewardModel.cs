using System;
using TonkoGames.Analytics;
using TonkoGames.StateMachine;
using Models.DataModels;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;

namespace Models.IAP
{
    public class IapRewardModel
    {
        private readonly IDataCentralService _dataCentralService;
        private readonly IWindowManager _windowManager;

        private IAPService _iapService;
        private IapModel _iapModel = null;
        public event Action<bool> RewardedBreakComplete;
        private readonly ReactiveProperty<bool> _canShowReward = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<int> _shownRewards = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<bool> CanShowReward => _canShowReward;
        public IReadOnlyReactiveProperty<int> ShownRewards => _shownRewards;
        
        private int _rewardsAvailable = 2;
        private const int MaxRewardAvailable = 2;
        private bool _timerIsActive = false;
        private CompositeDisposable _timerDisposable = new CompositeDisposable();

        public IapRewardModel(IapModel iapModel, IWindowManager windowManager, 
            IDataCentralService dataCentralService, IAPService iapService)
        {
            _windowManager = windowManager;
            _dataCentralService = dataCentralService;
            _iapModel = iapModel;
            _iapService = iapService;

        }

        public void Init()
        {
            _rewardsAvailable = MaxRewardAvailable;
            _iapModel.EndRewardedBreak += OnRewardedBreakComplete;
            _iapModel.CanShowReward.Subscribe(_ => SetCanShowReward());
        }
        
        public void ShowRewardedBreak()
        {
            if (_iapModel.IsAdblock())
            {
                _windowManager.Show<AdblockWindow>(WindowPriority.LoadScene);
                return;
            }
            
            if (_canShowReward.Value)
            {
                _rewardsAvailable--;
                _iapModel.ShowRewardedBreak();
                _iapService.SetBlocker(true);
            }
            else
            {
                RewardedBreakComplete?.Invoke(false);
            }
        }
        private void OnRewardedBreakComplete(bool value)
        {
            if (value)
            {
                _shownRewards.Value ++;
                GameAnalytics.Instance.PushEvent(StringsHelper.Analytics.iap, StringsHelper.Analytics.reward_shown);
                _dataCentralService.StatsDataModel.SetChestRewardsCount(_dataCentralService.StatsDataModel.ChestRewardsCount.Value + 1);
            }
            
            _iapService.SetBlocker(false);
            RewardedBreakComplete?.Invoke(value);
            
            RewardCdTimer();
        }
        
        private void RewardCdTimer()
        {
            if (_timerIsActive)
                return;
            _timerIsActive = true;
            Observable.Timer(TimeSpan.FromSeconds(40)).Repeat().Subscribe(_ =>
            {
                _rewardsAvailable++;
                SetCanShowReward();
                if (_rewardsAvailable >= MaxRewardAvailable)
                {
                    _timerIsActive = false;
                    _timerDisposable.Clear();
                }
            }).AddTo(_timerDisposable);
        }

        private void SetCanShowReward(){
            _canShowReward.Value = _rewardsAvailable > 0 && _iapModel.CanShowReward.Value;
        }
    }
}