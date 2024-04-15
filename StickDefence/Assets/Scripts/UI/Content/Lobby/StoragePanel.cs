using System;
using TonkoGames.Controllers.Core;
using Models.Controllers;
using Models.DataModels;
using Models.IAP;
using TMPro;
using Tools.Extensions;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Content.Lobby
{
    public class StoragePanel : UIBehaviour
    {
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private TMP_Text _currentCapacityLabel;
        [SerializeField] private Slider _fillCurrentCapacity;
        [SerializeField] private TMP_Text _capacityFullLabel;
        [SerializeField] private GameObject _fullStorageBlock;
        [SerializeField] private GameObject _addBlock;
        [SerializeField] private TMP_Text _addCapacityFull;
        [SerializeField] private UIButton _upgradeButton;
        [SerializeField] private UIButton _claimButton;
        [SerializeField] private UIButton _claimRewardButton;
        [SerializeField] private UIButton _closeButton;

        [Inject] private readonly ILobbyModels _lobbyModels;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly IIAPService _iapService;

        private CompositeDisposable _disposable = new();
        
        private int _currentCapacityFull = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            _dataCentralService.StatsDataModel.StorageLevel.Subscribe(SetupStorage).AddTo(_disposable);
            
            _lobbyModels.CoinFarmerModel.CanUpgradeStorage.Subscribe(value => _upgradeButton.IsInteractable = value);

            _upgradeButton.OnClickAsObservable.Subscribe(_ => _lobbyModels.CoinFarmerModel.UpgradeStorage()).AddTo(_disposable);
            
            _dataCentralService.StatsDataModel.StorageIncomeCount.SkipLatestValueOnSubscribe().Subscribe(SetFillCapacity).AddTo(_disposable);
            _claimButton.OnClickAsObservable.Subscribe(_ => _lobbyModels.CoinFarmerModel.ClaimStorage()).AddTo(_disposable);
            
            _iapService.CanShowReward.Subscribe(SetReward).AddTo(_disposable);
            
            _claimRewardButton.OnClick.AsObservable().Subscribe(_ => OnRewardClaim()).AddTo(_disposable);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            _disposable.Clear();
        }
        
        public void Open(Action closeAction)
        {
            gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(true);
            _closeButton.OnClickAsObservable.Subscribe(_ =>
            {
                gameObject.SetActive(false);
                closeAction?.Invoke();
            }).AddTo(_disposable);
        }

        private void SetupStorage(int level)
        {
            _levelLabel.text = level.ToString();
                    
            var currentStorage = _configManager.PumpingConfigSo.CoinFarmerConfigModel[level];
            _currentCapacityFull = currentStorage.StorageCapacity;
                    

            if (_lobbyModels.CoinFarmerModel.IsStorageIsMaxLevel)
            {
                _upgradeButton.gameObject.SetActive(false);
                _addBlock.SetActive(false);
            }
            else
            {
                _addBlock.SetActive(true);
                _upgradeButton.gameObject.SetActive(true);
                        
                _capacityFullLabel.text = SetScoreExt.ConvertIntToStringValue(_currentCapacityFull, 1);
                var nextStorage = _configManager.PumpingConfigSo.CoinFarmerConfigModel[level + 1];

                int deltaCapacity = nextStorage.StorageCapacity - _currentCapacityFull;
                _addCapacityFull.text = $"+{SetScoreExt.ConvertIntToStringValue(deltaCapacity, 1)}";
                _upgradeButton.Label.text = SetScoreExt.ConvertIntToStringValue(nextStorage.StorageCost, 1);
            }

            SetFillCapacity(_dataCentralService.StatsDataModel.StorageIncomeCount.Value);
        }
        
        private void OnRewardClaim()
        {
            _iapService.ShowRewardedBreak(value =>
            {
                if (value)
                {
                    _lobbyModels.CoinFarmerModel.ClaimX2Storage();
                }
            });
        }
        
        private void SetReward(bool canShowReward)
        {
            _claimRewardButton.gameObject.SetActive(canShowReward); 
        }
        
        private void SetFillCapacity(int currentCapacity)
        {
            _fillCurrentCapacity.value = (float) currentCapacity / (float) _currentCapacityFull;
            _currentCapacityLabel.text = $"{SetScoreExt.ConvertIntToStringValue(currentCapacity, 1)}/{SetScoreExt.ConvertIntToStringValue(_currentCapacityFull, 1)}";
            _fullStorageBlock.SetActive(currentCapacity >= _currentCapacityFull);
        }
    }
}