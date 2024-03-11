using System;
using TonkoGames.Controllers.Core;
using Models.Controllers;
using Models.DataModels;
using TMPro;
using Tools.Extensions;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.Lobby
{
    public class CoinSmeltingPanel : UIBehaviour
    {
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private TMP_Text _countPerTickLabel;
        [SerializeField] private TMP_Text _countPerTickCurrentLabel;
        [SerializeField] private GameObject _addBlock;
        [SerializeField] private TMP_Text _countPerTickAddLabel;
        [SerializeField] private UIButton _upgradeButton;
        [SerializeField] private UIButton _closeButton;

        [Inject] private readonly ILobbyModels _lobbyModels;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly IWindowManager _windowManager;

        private CompositeDisposable _disposable = new();

        protected override void OnEnable()
        {
            base.OnEnable();

            _dataCentralService.StatsDataModel.CoinSmeltingLevel.Subscribe(SetupSettings).AddTo(_disposable);
            _lobbyModels.CoinFarmerModel.CanUpgradeCoinSmelting.Subscribe(value => _upgradeButton.IsInteractable = value);

            _upgradeButton.OnClickAsObservable.Subscribe(_ => _lobbyModels.CoinFarmerModel.UpgradeCoinSmelting()).AddTo(_disposable);
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
        
        private void SetupSettings(int level)
        {
            _levelLabel.text = level.ToString();
            var current = _configManager.PumpingConfigSo.CoinFarmerConfigModel[level];
            _countPerTickCurrentLabel.text = _countPerTickLabel.text = SetScoreExt.ConvertIntToStringValue(current.CoinSmeltingGoldTick, 1);

            if (_lobbyModels.CoinFarmerModel.IsCoinSmeltingIsMaxLevel)
            {
                _upgradeButton.gameObject.SetActive(false);
                _addBlock.SetActive(false);
            }
            else
            {
                _upgradeButton.gameObject.SetActive(true);
                _addBlock.SetActive(true);
                var nextSmelting = _configManager.PumpingConfigSo.CoinFarmerConfigModel[level + 1];

                int deltaTick = nextSmelting.CoinSmeltingGoldTick - current.CoinSmeltingGoldTick;
                _countPerTickAddLabel.text = $"+{SetScoreExt.ConvertIntToStringValue(deltaTick, 1)}";
                _upgradeButton.Label.text = SetScoreExt.ConvertIntToStringValue(nextSmelting.CoinSmeltingCost, 1);
            }
        }
    }
}