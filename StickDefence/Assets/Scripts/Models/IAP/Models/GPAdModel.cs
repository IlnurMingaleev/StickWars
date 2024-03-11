using System;
using I2.Loc;
using Models.IAP.InApps;
using UI.Windows;
using UnityEngine;

namespace Models.IAP.Models
{
#if UNITY_WEBGL || UNITY_EDITOR
    using GamePush;
    public class GPAdModel : IapModel
    {
        public GPAdModel(IAPService iapService) : base(iapService)
        {
        }
        
        public override bool IsLogin => GP_Player.IsLoggedIn();

        public override bool IsAdblock()
        {
            return GP_Ads.IsAdblockEnabled();
        }
        
        public override void SetGamePlayStart()
        {
            GP_Game.GameplayStart();
        }
        
        public override void SetGamePlayStop()
        {
            GP_Game.GameplayStop();
        }
        
        public override void ShowCommercialBreak()
        {
            base.ShowCommercialBreak();
            GP_Ads.OnFullscreenClose += CommercialBreakComplete;
            GP_Ads.ShowFullscreen();
            CanShowInter.Value = false;
        }
        
        public override void ShowRewardedBreak()
        {
            base.ShowRewardedBreak();
            CanShowReward.Value = false;
            GP_Ads.OnRewardedClose += RewardedBreakComplete;
            GP_Ads.ShowRewarded();
        }

        private void CommercialBreakComplete(bool arg0)
        {
            CanShowInter.Value = true;
            OnCommercialBreakComplete();
            GP_Ads.OnFullscreenClose -= CommercialBreakComplete;
        }
        
        private void RewardedBreakComplete(bool value)
        {
            if (value)
            {
                OnRewardedBreakComplete(true);
                GP_Ads.OnRewardedClose -= RewardedBreakComplete;
            }
            else
            {
                OnRewardedBreakComplete(false);
                GP_Ads.OnRewardedClose -= RewardedBreakComplete;
            }
            CanShowReward.Value = true;
        }

        public override void TryLogin()
        {
            Iap.WindowManager.Show<ChoiceMessageWindow>().Init(ScriptLocalization.Messages.NeedLogin, GP_Player.Login);
        }

        public override void ShowStickyBanner(bool value)
        {
            if (value)
            {
                GP_Ads.ShowSticky();
            }
            else
            {
                GP_Ads.CloseSticky();
            }
        }
    }
#endif
}