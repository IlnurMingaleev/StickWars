using System;
using TonkoGames.Controllers.Core;
using Models.DataModels;
using Models.IAP.Models;
using UI.Windows;
using UniRx;
using UnityEngine;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.IAP.InApps;
using UI.UIManager;

namespace Models.IAP
{
    public interface IIAPService 
    {
        event Action CommercialBreakComplete;
        bool ShowCommercialBreak();
        void ShowRewardedBreak(Action<bool> rewardedBreakAction);
        IReadOnlyReactiveProperty<bool> CanShowReward { get; }
        void HappyTime();
        IPaymentModel PaymentModel { get; }
        string GetUID();
        void TryCollectNonCollectedInApps();
    }
    public class IAPService : IIAPService
    {
        private IapModel _iapModel = null;
        private float _lastSoundValue = 0;
        private float _lastMusicValue = 0;
        public readonly IWindowManager WindowManager;
        private readonly ICoreStateMachine _coreStateMachine;
        private readonly IDataCentralService _dataCentralService;
        private readonly InAppSystem _inAppSystem;
        public IReadOnlyReactiveProperty<bool> CanShowReward => _iapRewardModel.CanShowReward;

        private IapRewardModel _iapRewardModel;
        private IapInterstitialModel _iapInterstitialModel;
        private PaymentModel _paymentModel;

        public IPaymentModel PaymentModel => _paymentModel;
        public IapModel IapModel => _iapModel;

        private Action<bool> RewardedBreakAction; 
        
        public IAPService(IWindowManager windowManager, ICoreStateMachine coreStateMachine, 
            IDataCentralService dataCentralService, ConfigManager configManager)
        {
            WindowManager = windowManager;
            _coreStateMachine = coreStateMachine;
            _dataCentralService = dataCentralService;
            _inAppSystem = new InAppSystem(windowManager, configManager, dataCentralService, this);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _iapModel = new DevAddModel(this);
            _paymentModel = new DevPaymentModel(this, _dataCentralService, _inAppSystem);
#elif UNITY_WEBGL
            _iapModel = new GPAdModel(this);
            _paymentModel = new WebPaymentModel(this, _dataCentralService, _inAppSystem);

  if (GamePush.GP_Ads.CanShowFullscreenBeforeGamePlay())
             {
                 GamePush.GP_Ads.OnPreloaderStart += PreLoaderStart;
                 GamePush.GP_Ads.OnPreloaderClose += PreloaderClose;

                 void PreLoaderStart()
                 {
                     GamePush.GP_Ads.OnPreloaderStart -= PreLoaderStart;
                     AudioListener.pause = true;
                 }
            
                 void PreloaderClose(bool value)
                 {
                     if (value)
                     {
                         GamePush.GP_Ads.OnPreloaderStart -= PreLoaderStart;
                         GamePush.GP_Ads.OnPreloaderClose -= PreloaderClose;
                         AudioListener.pause = false;
                     }
                 }
             }
#elif UNITY_ANDROID || UNITY_IPHONE
            _iapModel = new MobileAddModel(this);
            _paymentModel = new AndroidPaymentModel(this, _dataCentralService, _inAppSystem);
#endif
            _iapRewardModel = new IapRewardModel(_iapModel, WindowManager, _dataCentralService, this);
            _iapInterstitialModel = new IapInterstitialModel(_iapModel, this);
            
            SetGameLoadingStart();
            
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Play, () => _iapModel.SetGamePlayStart());
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Pause, () => _iapModel.SetGamePlayStop());
            _iapRewardModel.Init();
            _iapInterstitialModel.Init();
            _iapRewardModel.RewardedBreakComplete += OnRewardedBreakComplete;
            _iapInterstitialModel.CommercialBreakComplete += OnCommercialBreakComplete;
            _dataCentralService.SubData.IsADSRemove.Subscribe(value =>
            {
                _iapInterstitialModel.SetIsADSRemove(value);
                _iapModel.ShowStickyBanner(!value);
            });
        }

        public void TryCollectNonCollectedInApps()
        {
            _inAppSystem.TryCollectNonCollectedInApps().Forget();
        }
        
        public string GetUID()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return GamePush.GP_Player.GetID().ToString();
#endif
            return string.Empty;
        }
        public event Action CommercialBreakComplete;

        public void SetGameLoadingStart() => _iapModel.SetGameLoadingStart();   
        
        public void SetGameLoadingFinished() => _iapModel.SetGameLoadingFinished();

        public void HappyTime()
        {
#if UNITY_WEBGL
            GamePush.GP_Game.HappyTime();
#endif
        }

        #region CommercialBreaks
        public bool ShowCommercialBreak()
        {
#if UNITY_EDITOR  || DEVELOPMENT_BUILD
            Debug.Log("Inter UNITY_EDITOR");
            CommercialBreakComplete?.Invoke();
            return false;
#else
            return _iapInterstitialModel.OnCommercialBreak();
#endif
        }

        public void ShowRewardedBreak(Action<bool> rewardedBreakAction)
        {
            RewardedBreakAction = rewardedBreakAction;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Reward DEVELOPMENT_BUILD");
            RewardedBreakAction?.Invoke(true);
            _dataCentralService.StatsDataModel.SetChestRewardsCount(_dataCentralService.StatsDataModel.ChestRewardsCount.Value + 1);
#else
        _iapRewardModel.ShowRewardedBreak();
#endif
            
        }

        private void OnRewardedBreakComplete(bool value){
            RewardedBreakAction?.Invoke(value);
            RewardedBreakAction = null;
        }
        
        private void OnCommercialBreakComplete() => CommercialBreakComplete?.Invoke();
        #endregion

        
        public void SetBlocker(bool value)
        {
            if (value)
            {
                _lastSoundValue = _dataCentralService.SubData.SoundVolume.Value;
                _lastMusicValue = _dataCentralService.SubData.MusicVolume.Value;
                WindowManager.Show<WaitingWindow>(WindowPriority.LoadScene);
                _dataCentralService.SubData.SetSoundVolume(0);
                _dataCentralService.SubData.SetMusicVolume(0);
                _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Pause);
            }
            else
            {
                WindowManager.Hide<WaitingWindow>();
                _dataCentralService.SubData.SetSoundVolume(_lastSoundValue);
                _dataCentralService.SubData.SetMusicVolume(_lastMusicValue);
                _coreStateMachine.RunTimeStateMachine.SetRunTimeState(_coreStateMachine.RunTimeStateMachine.LastRunTimeState);
            }
        }
    }
}