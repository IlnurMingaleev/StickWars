using I2.Loc;
using Models.IAP.InApps;
using Tools.Extensions;

namespace UI.Content.Shop
{
    public class UIGemPurchase : PaymentBlock
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            OnLocalizeEvent();
            
            _valueLabel.text = SetScoreExt.ConvertIntToStringValue(_ConfigManager.IapSO.AppRewards[_paymentProductType].Gem, 1);
        }
        
        protected override void OnLocalizeEvent()
        {
            _priceLabel.text = SetScoreExt.ConvertIntToStringValue(_inAppSettings.Price, 1) + " " +
                               _inAppSettings.RealCurrencyType.ToTranslatedName();
        }
    }
}