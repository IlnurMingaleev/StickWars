using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using UniRx;
using UnityEngine;
namespace Models.IAP.Models
{

    public class MobileAddModel //: IapModel
    {
/*#if UNITY_EDITOR || DEVELOPMENT_BUILD
#if UNITY_ANDROID
//TODO: REmove back
#elif UNITY_IPHONE
//TODO: REmove back
#else
            private const string _rewardAdUnitId = "unused";
            private const string _interAdUnitId = "unused";
#endif
    
#else
//TODO: REmove back
#endif 
        
        public MobileAddModel(IAPService iapService) : base(iapService)
        {
            MobileAds.SetiOSAppPauseOnBackground(true);

            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
            
            // Add some test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
        //deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
//TODO: REmove back
#endif

            // Configure TagForChildDirectedTreatment and test device IDs.
            RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
                    .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                    .SetTestDeviceIds(deviceIds).build();
            
            
            MobileAds.SetRequestConfiguration(requestConfiguration);
            Debug.Log("Google Mobile Ads Initializing.");
            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                if (initstatus == null)
                {
                    Debug.LogError("Google Mobile Ads initialization failed.");
                    return;
                }

                Debug.Log("Google Mobile Ads initialization complete.");

                LoadRewardAd();
                LoadInterAd();
            });
        }

        #region Reward
        
        private ReactiveProperty<RewardedAd> _rewardedAd = new ReactiveProperty<RewardedAd>();
        private bool _loadingReward = false;

        private void LoadWaiterReward()
        {
            _loadingReward = true;
            Observable.Timer (System.TimeSpan.FromMinutes(1), Scheduler.MainThreadIgnoreTimeScale)
                .Subscribe (_ =>
                {
                    LoadRewardAd();
                    _loadingReward = false;
                }); 
        }
        private async void LoadRewardAd()
        {
            if (_rewardedAd.Value != null)
            {
                Debug.Log("Destroying rewarded ad.");
                _rewardedAd.Value.Destroy();
                _rewardedAd.Value = null;
                CanShowReward.Value = false;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
            Debug.Log("Loading rewarded ad.");

            RewardedAd.Load(_rewardAdUnitId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                    LoadWaiterReward();
                    return;
                }
                
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                    LoadWaiterReward();
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                _rewardedAd.Value = ad;
                CanShowReward.Value = true;

                RegisterRewardEventHandlers(ad);
            });
        }


        public override void ShowRewardedBreak()
        {
            if (_rewardedAd.Value != null && _rewardedAd.Value.CanShowAd())
            {
                CanShowReward.Value = false;
                Debug.Log("Showing rewarded ad.");
                _rewardedAd.Value.Show((Reward reward) =>
                {
                    Debug.Log(String.Format("Rewarded ad granted a reward: {0} {1}",
                                            reward.Amount,
                                            reward.Type));
                });
            }
            else
            {
                OnRewardedBreakComplete(false);
                if (!_loadingReward)
                {
                    LoadRewardAd();
                }
                Debug.LogError("Rewarded ad is not ready yet.");
            }
        }


        private void RegisterRewardEventHandlers(RewardedAd ad)
        {
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                OnRewardedBreakComplete(true);
                Debug.Log("Rewarded ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                OnRewardedBreakComplete(false);
                Debug.LogError("Rewarded ad failed to open full screen content with error : "
                    + error);
            };
        }

        #endregion

        #region Inter
        
        private ReactiveProperty<InterstitialAd> _interstitialAd = new ReactiveProperty<InterstitialAd>();
        private bool _loadingInter = false;

        private void LoadWaiterInter()
        {
            _loadingInter = true;
            Observable.Timer (System.TimeSpan.FromMinutes(1), Scheduler.MainThreadIgnoreTimeScale)
                .Subscribe (_ =>
                {
                    LoadInterAd();
                }); 
        }
        
        private async void LoadInterAd()
        {
            if (_interstitialAd.Value != null)
            {
                Debug.Log("Destroying interstitial ad.");
                _interstitialAd.Value.Destroy();
                _interstitialAd.Value = null;
                CanShowInter.Value = false;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
            Debug.Log("Loading interstitial ad.");

            var adRequest = new AdRequest();

            InterstitialAd.Load(_interAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                    LoadWaiterInter();
                    return;
                }
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                    LoadWaiterInter();
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                _interstitialAd.Value = ad;
                CanShowInter.Value = true;
                _loadingInter = false;

                RegisterInterEventHandlers(ad);
            });
        }

        public override void ShowCommercialBreak()
        {
            if (_interstitialAd.Value != null && _interstitialAd.Value.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Value.Show();
                CanShowInter.Value = false;
            }
            else
            {
                OnCommercialBreakComplete();
                if (!_loadingInter)
                {
                    LoadInterAd();
                }
                Debug.LogError("Interstitial ad is not ready yet.");
            }
        }

        private void RegisterInterEventHandlers(InterstitialAd ad)
        {
            ad.OnAdFullScreenContentClosed += () =>
            {
                CanShowInter.Value = false;
                OnCommercialBreakComplete();
                Debug.Log("Interstitial ad full screen content closed.");
            };
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                OnCommercialBreakComplete();
                LoadInterAd();
                Debug.LogError("Interstitial ad failed to open full screen content with error : "
                    + error);
            };
        }

        #endregion
#endregion*/
    }

}