using TonkoGames.Controllers.Core;
using Models.DataModels;
using UniRx;

namespace Models.Lobby
{
    public interface ICoinFarmerModel
    {
        void UpgradeStorage();
        void ClaimStorage();
        void ClaimX2Storage();
        void UpgradeCoinSmelting();
        bool IsCoinSmeltingIsMaxLevel { get; }
        bool IsStorageIsMaxLevel { get; }
        IReadOnlyReactiveProperty<bool> CanUpgradeCoinSmelting { get; }
        IReadOnlyReactiveProperty<bool> CanUpgradeStorage { get; }
        IReadOnlyReactiveProperty<bool> CanCollectIncomeCoins { get; }
    }

    public class CoinFarmerModel : ICoinFarmerModel
    {
        private CompositeDisposable _timerDisposable = new CompositeDisposable();

        private int _coinSmeltingTickCoin = 0;
        private int _storageCapacity = 0;

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private ReactiveProperty<bool> _canUpgradeCoinSmelting = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _canUpgradeStorage = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _canCollectIncomeCoins = new ReactiveProperty<bool>();

        private int _forgeTickRate = 5; //Тик каждые 5 секунд
        
        public IReadOnlyReactiveProperty<bool> CanUpgradeCoinSmelting => _canUpgradeCoinSmelting;
        
        public IReadOnlyReactiveProperty<bool> CanUpgradeStorage => _canUpgradeStorage;
        public IReadOnlyReactiveProperty<bool> CanCollectIncomeCoins => _canCollectIncomeCoins;
        
        private readonly IDataCentralService _dataCentralService;
        private readonly ConfigManager _configManager;
        
        public bool IsCoinSmeltingIsMaxLevel => _dataCentralService.StatsDataModel.CoinSmeltingLevel.Value ==
                                                 _configManager.PumpingConfigSo.CoinFarmerConfigModel.Count - 1;
        public bool IsStorageIsMaxLevel => _dataCentralService.StatsDataModel.StorageLevel.Value ==
                                           _configManager.PumpingConfigSo.CoinFarmerConfigModel.Count - 1;
        
        public CoinFarmerModel(IDataCentralService dataCentralService, ConfigManager configManager)
        {
            _dataCentralService = dataCentralService;
            _configManager = configManager;
        }

        public void Init()
        {
            _dataCentralService.StatsDataModel.StorageLevel.Subscribe(value =>
            {
                _storageCapacity = 
                    _configManager.PumpingConfigSo.CoinFarmerConfigModel[value].StorageCapacity;
                OnCanUpgradeCoinSmelting(value, _dataCentralService.StatsDataModel.CoinsCount.Value);
            }).AddTo(_compositeDisposable);
            
            _dataCentralService.StatsDataModel.CoinSmeltingLevel.Subscribe(value =>
            {
                _coinSmeltingTickCoin =
                    _configManager.PumpingConfigSo.CoinFarmerConfigModel[value].CoinSmeltingGoldTick;
                OnCanUpgradeCoinSmelting(value, _dataCentralService.StatsDataModel.CoinsCount.Value);
            }).AddTo(_compositeDisposable);
            
            _dataCentralService.StatsDataModel.CoinsCount.SkipLatestValueOnSubscribe().Subscribe(value =>
            {
                OnCanUpgradeCoinSmelting(_dataCentralService.StatsDataModel.CoinSmeltingLevel.Value, value);
                OnCanUpgradeStorage(_dataCentralService.StatsDataModel.StorageLevel.Value, value);
            }).AddTo(_compositeDisposable);
            
            StartCoinSmelting();
            _dataCentralService.StatsDataModel.StorageIncomeCount.Subscribe(CoinsIncomeCountChange).AddTo(_compositeDisposable);
        }
        
        private void OnCanUpgradeCoinSmelting(int workshopLevel, long coinsCount) => _canUpgradeCoinSmelting.Value = !IsCoinSmeltingIsMaxLevel &&
            coinsCount >= _configManager.PumpingConfigSo.CoinFarmerConfigModel[workshopLevel + 1].CoinSmeltingCost;
        private void OnCanUpgradeStorage(int storageLevel, long coinsCount) => _canUpgradeStorage.Value = !IsStorageIsMaxLevel &&
            coinsCount >= _configManager.PumpingConfigSo.CoinFarmerConfigModel[storageLevel + 1].StorageCost;
        private void CoinsIncomeCountChange(int value) => _canCollectIncomeCoins.Value = value > 0;
        private void StartCoinSmelting()
        {
            _timerDisposable.Clear();
            
            Observable.Timer (System.TimeSpan.FromSeconds(_forgeTickRate))
                .Repeat()
                .Subscribe (_ =>
                {
                    AddStorageIncome(_coinSmeltingTickCoin);
                }).AddTo (_timerDisposable);
        }

        private void AddStorageIncome(int count)
        {
            if (_dataCentralService.StatsDataModel.StorageIncomeCount.Value < _storageCapacity)
            {
                int tmpCoinsIncome = _dataCentralService.StatsDataModel.StorageIncomeCount.Value + count;
                if (tmpCoinsIncome > _storageCapacity)
                {
                    tmpCoinsIncome = _storageCapacity;
                }
                        
                _dataCentralService.StatsDataModel.SetStorageIncomeCount(tmpCoinsIncome);
            }
        }

        public void UpgradeStorage()
        {
            _dataCentralService.StatsDataModel.MinusCoinsCount(
                _configManager.PumpingConfigSo.CoinFarmerConfigModel[_dataCentralService.StatsDataModel.StorageLevel.Value + 1].StorageCost);
            _dataCentralService.StatsDataModel.SetStorageLevel(
                _dataCentralService.StatsDataModel.StorageLevel.Value + 1);
            _dataCentralService.SaveFull();
        }
        
        public void UpgradeCoinSmelting()
        {
            _dataCentralService.StatsDataModel.MinusCoinsCount(_configManager.PumpingConfigSo.CoinFarmerConfigModel[_dataCentralService.StatsDataModel.CoinSmeltingLevel.Value + 1].CoinSmeltingCost);
            _dataCentralService.StatsDataModel.SetCoinSmeltingLevel(
                _dataCentralService.StatsDataModel.CoinSmeltingLevel.Value + 1);
            _dataCentralService.SaveFull();
        }

        public void ClaimStorage() => ClaimStorage(_dataCentralService.StatsDataModel.StorageIncomeCount.Value);

        public void ClaimX2Storage()
        {
            ClaimStorage(_dataCentralService.StatsDataModel.StorageIncomeCount.Value * 2);
        }

        private void ClaimStorage(int addCoins)
        {
            _dataCentralService.StatsDataModel.AddCoinsCount(addCoins);
            _dataCentralService.StatsDataModel.SetStorageIncomeCount(0);
            _dataCentralService.SaveFull();
        }
    }
}