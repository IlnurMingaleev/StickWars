using System;
using Enums;
using Models.IAP.InApps;
using UniRx;
using UnityEngine;

namespace Models.IAP
{
    public abstract class IapModel
    {
        public event Action EndCommercialBreak;
        public event Action<bool> EndRewardedBreak;
#if UNITY_EDITOR || DEVELOPMENT_BUILD || UNITY_WEBGL
        public readonly ReactiveProperty<bool> CanShowReward = new ReactiveProperty<bool>(true);
        public readonly ReactiveProperty<bool> CanShowInter = new ReactiveProperty<bool>(true);
#else 
        public readonly ReactiveProperty<bool> CanShowReward = new ReactiveProperty<bool>(false);
        public readonly ReactiveProperty<bool> CanShowInter = new ReactiveProperty<bool>(false);
#endif

        protected readonly IAPService Iap;

        protected IapModel(IAPService iapService)
        {
            Iap = iapService;
        }


        public virtual bool IsLogin { get; protected set; } = false;
        
        public virtual bool IsAdblock()
        {
            return false;
        }
        
        public virtual void SetGamePlayStart()
        {
        }
        public virtual void SetGamePlayStop()
        {
        }
        
        public virtual void SetGameLoadingStart()
        {
        }
        
        public virtual void SetGameLoadingFinished()
        {
        }
        
        public virtual void ShowCommercialBreak()
        {
            Debug.Log("Show CommercialBreak");
        }
        
        public virtual void ShowRewardedBreak()
        {
            Debug.Log("Show RewardedBreak");
        }
        
        protected void OnCommercialBreakComplete()
        {
            EndCommercialBreak?.Invoke();
            Debug.Log("Commercial break finished");
        }
        protected void OnRewardedBreakComplete(bool withReward){
            EndRewardedBreak?.Invoke(withReward);
            Debug.Log("Rewarded break finished, should i get a reward:" + withReward.ToString());
        }

        public virtual void TryLogin() { }
        
        public virtual void ShowStickyBanner(bool value) { }
    }
}