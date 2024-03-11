using I2.Loc;
using Models.IAP.InApps;
using Tools.Extensions;

namespace UI.Content.Shop
{
    public class RemoveADS : PaymentBlock
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            OnLocalizeEvent();
        }
        
        protected override void OnLocalizeEvent()
        {
            _priceLabel.text = ScriptLocalization.Windows_Shop.RemoveADS + " " + SetScoreExt.ConvertIntToStringValue(_inAppSettings.Price, 1) + " " +
                               _inAppSettings.RealCurrencyType.ToTranslatedName();
        }
    }
}