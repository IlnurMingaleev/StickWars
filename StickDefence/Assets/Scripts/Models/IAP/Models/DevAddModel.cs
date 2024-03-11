using GamePush;
using I2.Loc;
using UI.Windows;
using UnityEngine;

namespace Models.IAP.Models
{
    public class DevAddModel : IapModel
    {
        public override bool IsLogin { get; protected set; } = true;
        
        public DevAddModel(IAPService iapService) : base(iapService)
        {
            CanShowInter.Value = true;
            CanShowReward.Value = true;
        }
        
        public override void TryLogin()
        {
            Iap.WindowManager.Show<ChoiceMessageWindow>().Init(ScriptLocalization.Messages.NeedLogin, () => IsLogin = true);
        }
    }
}