using System;
using TonkoGames.Analytics;
using TonkoGames.StateMachine;
using Tools.GameTools;
using UniRx;

namespace Models.IAP
{
    public class IapInterstitialModel
    {
        private IAPService _iapService;
        private IapModel _iapModel = null;
        private bool _isADSRemove = false;
        public event Action CommercialBreakComplete;
        
        private readonly ReactiveProperty<bool> _canShowInter = new ReactiveProperty<bool>(false);

        private int _tempCallsToShow = 0;
#if UNITY_EDITOR || UNITY_WEBGL
        private const int MaxCallsToShow = 2;
#else
        private const int MaxCallsToShow = 2;
#endif

        private bool _interCD = false;
        
        public IapInterstitialModel(IapModel iapModel, IAPService iapService)
        {
            _iapModel = iapModel;
            _iapService = iapService;
        }

        public void Init()
        {
            _iapModel.EndCommercialBreak += OnCommercialBreakComplete;
            _tempCallsToShow = MaxCallsToShow;
        }

        public void SetIsADSRemove(bool value) => _isADSRemove = value;
        
        public bool OnCommercialBreak()
        {
            if (_interCD)
            {
                return false;
            }
            
            _tempCallsToShow--;
            
            if (_iapModel.IsAdblock() || !_iapModel.CanShowInter.Value || _tempCallsToShow > 0 || _isADSRemove)
            {
                CommercialBreakComplete?.Invoke();
                return false;
            }
            else
            {
                _iapModel.ShowCommercialBreak();
                _iapService.SetBlocker(true);
                return true;
            }
        }
        private void OnCommercialBreakComplete()
        {
            GameAnalytics.Instance.PushEvent(StringsHelper.Analytics.iap, StringsHelper.Analytics.interstitial_shown);
            _iapService.SetBlocker(false);
            CommercialBreakComplete?.Invoke();
            _tempCallsToShow = MaxCallsToShow;
            _interCD = true;
            
            Observable.Timer(TimeSpan.FromSeconds(60)).Subscribe(_ =>
            {
                _interCD = false;
            });
        }
    }
}