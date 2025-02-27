﻿using TonkoGames.StateMachine;
using Models.DataModels;
using TMPro;
using UI.Common;
using UI.Content.TopPanel;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Windows
{
    public class TopPanelWindow : Window
    {
        [SerializeField] private UIButton _backArrow;
        [SerializeField] private UIButton _settingsButton;
        [SerializeField] private UIButton _buttonExit;
        [SerializeField] private FinanceBar _financeBar;
        [SerializeField] private GameObject _statusBar;
        [SerializeField] private Transform _playerRespect;
        [SerializeField] private UIBar _experienceBar;
        [SerializeField] private TMP_Text _levelNumber;
        [SerializeField] private UIBar _levelProgressBar;
        [SerializeField] private TMP_Text _waveNumber;
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private ICoreStateMachine _coreStateMachine;
        private CompositeDisposable _openShopDisposable = new CompositeDisposable();
        
        public Transform CashLabelTransform => _financeBar.CoinLabelTransform;
        public Transform GoldLabelTransform => _financeBar.GemLabelTransform;
        public Transform RespectLabelTransform => _playerRespect;
        public UIButton ButtonExit => _buttonExit;
        public UIBar ExperienceBar => _experienceBar; 
        public UIBar LevelProgressBar => _levelProgressBar;
        protected override bool DisableMultiTouchOnShow => false;
        
        
        protected override void OnActivate()
        {
            base.OnActivate();
            SubscribeToExpLvl();
            /*_dataCentralService.PumpingDataModel.LevelReactive.Subscribe(_ => _levelNumber.text = $"{_.Level}")
                .AddTo(ActivateDisposables);*/
            _dataCentralService.PumpingDataModel.StageLoadType.Subscribe(_ => _waveNumber.text = $"{(int) _}")
                .AddTo(ActivateDisposables);
            _backArrow.OnClickAsObservable.Subscribe(_ => ExitGameToMainMenu()).AddTo(ActivateDisposables);
            _financeBar.SetAction(ShowShopWindow, ShowShopWindow);
            _manager.LastWindowsCount.Subscribe(CheckBackButtonActive).AddTo(ActivateDisposables);
            _settingsButton.OnClickAsObservable.Subscribe(_ => _manager.Show<SettingsWindow>(WindowPriority.AboveTopPanel)).AddTo(ActivateDisposables);
        }

        private void SubscribeToExpLvl()
        {
            _dataCentralService.PumpingDataModel.LevelReactive.Subscribe(levelData =>
            {
                ExperienceBar.SetBarFiilAmount(levelData.CurrentExp, levelData.RequiredExp);
            }).AddTo(ActivateDisposables);
        }

        public void SetTopGameState()
        {
            _backArrow.gameObject.SetActive(true);
            _statusBar.SetActive(false);
            _levelProgressBar.gameObject.SetActive(true);
        }
        
        public void SetTopLobbyState()
        {
            _statusBar.SetActive(true);
            _buttonExit.gameObject.SetActive(false);
            _levelProgressBar.gameObject.SetActive(false);
        }

        private void ShowShopWindow()
        {
            _manager.Hide(_manager.GetLastWindow(false));
            
            _manager.Show<ShopWindow>().IsShowingReactive.Subscribe(value =>
            {
                _financeBar.IsShopButtonVisible = !value;
                if (!value)
                {
                    _openShopDisposable.Clear();
                }
            }).AddTo(_openShopDisposable);
        }

        public void ChowSettingsButton(bool value)
        {
            _settingsButton.gameObject.SetActive(value);
        }

        private void OnBackArrowClick()
        {
            Window currentWindow = _manager.GetLastWindow();
            Window lastWindow = _manager.GetLastWindow();
            _manager.Show(lastWindow);
            _manager.Hide(currentWindow);
        }

        private void ExitGameToMainMenu()
        {
            _coreStateMachine.BattleStateMachine.OnEndBattle(false,true);
        }

        public void SetIgnoreNextProfileChange()
        {
            _financeBar.SetIgnoreNextProfileChange();
        }

        public void PlayProfileChangedAnimation()
        {
            _financeBar.PlayProfileChangedAnimation();
        }

        public void SkipProfileChangedAnimation()
        {
            _financeBar.SkipProfileChangedAnimation();
        }
        
        private void CheckBackButtonActive(int value) => _backArrow.gameObject.SetActive(value > 1);
    }
}