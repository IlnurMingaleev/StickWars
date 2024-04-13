using Enums;
using TonkoGames.Controllers.Core;
using Models.Controllers;
using Models.DataModels;
using Models.Timers;
using UniRx;

namespace Models.Player
{
    public interface IPlayer
    {
        IPumping Pumping { get; }
        IDailyModel DailyModel { get; }
        IReadOnlyReactiveProperty<int> SubscribeToCurrencyBuyType(CurrencyTypeEnum currencyTypeEnum);
        void ChangeCurrencyBuyType(CurrencyTypeEnum currencyTypeEnum, int count);
    }

    public interface IPlayerRoot : IPlayer
    {
        void Init();
        void Reinit();
        void CheckDailyModel();
    }
    public class Player : IPlayerRoot
    {
        private readonly IDataCentralService _dataCentralService;
        private readonly ConfigManager _configManager;

        private Pumping _pumping;
        
        private ReactiveProperty<int> _silverCurrency = new ReactiveProperty<int>();

       
        
        private DailyModel _dailyModel;
        public IDailyModel DailyModel => _dailyModel;
        public IPumping Pumping => _pumping;
        public Player(IDataCentralService dataCentralService, ConfigManager configManager, ITimerService timerService)
        {
            _dataCentralService = dataCentralService;
            _configManager = configManager;
            _dailyModel = new DailyModel(dataCentralService, configManager, timerService);
            _pumping = new Pumping(configManager, dataCentralService);
        }
        public void Init()
        {
            _pumping.Init();
        }
        public void Reinit()
        {
        }

       
        
        public void CheckDailyModel()
        {
            _dailyModel.CheckDailyModel();
            _dataCentralService.StatsDataModel.SetLastDataTimeVisit();
        }

        public IReadOnlyReactiveProperty<int> SubscribeToCurrencyBuyType(CurrencyTypeEnum currencyTypeEnum)
        {
            return currencyTypeEnum switch
            {
                CurrencyTypeEnum.Gold => _dataCentralService.StatsDataModel.CoinsCount,
                CurrencyTypeEnum.Gem => _dataCentralService.StatsDataModel.GemsCount,
                CurrencyTypeEnum.Silver => _silverCurrency,
                _ => null
            };
        }

        public void ChangeCurrencyBuyType(CurrencyTypeEnum currencyTypeEnum, int count)
        {
            switch (currencyTypeEnum)
            {
                case CurrencyTypeEnum.Gold:
                    if (count > 0)
                        _dataCentralService.StatsDataModel.AddCoinsCount(count);
                    else
                        _dataCentralService.StatsDataModel.MinusCoinsCount(-count);
                    break;
                case CurrencyTypeEnum.Gem:
                    if (count > 0)
                        _dataCentralService.StatsDataModel.AddGemsCount(count);
                    else
                        _dataCentralService.StatsDataModel.MinusGemsCount(-count);
                    break;
                case CurrencyTypeEnum.Silver:
                    _silverCurrency.Value += count;
                    break;
            }
            
            _dataCentralService.SaveFull();
        }
    }
}