using System;
using System.Collections.Generic;
using I2.Loc;
using Models.Controllers;
using Models.DataModels;
using Models.IAP;
using TMPro;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using UI.Common;
using UI.Content.PlayResult;
using UI.Content.Rewards;
using UI.UIManager;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Windows
{
    public class PlayResultWindow : Window
    {
        [SerializeField] private GameObject _resurrectBlock;
        [SerializeField] private GameObject _winBlock;
        [SerializeField] private GameObject _exitGameBlock;

        [Header("WinBlock")]
        [SerializeField] private TMP_Text _title;
        [SerializeField] private ResultRewardBlock _coinRewardCountBlock;
        [SerializeField] private ResultRewardBlock _gemRewardCountBlock;
        [SerializeField] private UIButton _claimButton;
        [SerializeField] private UIButton _rewardClaimButton;
        [SerializeField] private GameObject _starsBlock;
        [SerializeField] private List<GameObject> _stars;
        [Header("Resurrect")]
        [SerializeField] private UIButton _rewardResurrectButton;
        [SerializeField] private UIButton _defeatButton;
        [Header("ExitGameBlock")]
        [SerializeField] private UIButton _returnBackToGame;
        [SerializeField] private UIButton _exitGameButton;

        [Inject] private readonly IIAPService _iapService;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;

        private Action _claim;
        private Action _resurrect;
        private Action _continue;
        private Action _upgradeStageIndex;

        private int _coinsCount = 0;
        private int _gemCount = 0;
        private bool _rewarded = false;
        private bool _isWin = false; 
        
        protected override void OnActivate()
        {
            base.OnActivate();
            _rewarded = false;
            _iapService.CanShowReward.Subscribe(SetReward).AddTo(ActivateDisposables);

            _claimButton.OnClick.AsObservable().Subscribe(_ => OnClaim()).AddTo(ActivateDisposables);
            _rewardClaimButton.OnClick.AsObservable().Subscribe(_ => OnRewardClaim()).AddTo(ActivateDisposables);
            
            _defeatButton.OnClick.AsObservable().Subscribe(_ => OnDefeatButton()).AddTo(ActivateDisposables);
            _rewardResurrectButton.OnClick.AsObservable().Subscribe(_ => OnResurrect()).AddTo(ActivateDisposables);

            _returnBackToGame.OnClickAsObservable.Subscribe(_ => OnReturn()).AddTo(ActivateDisposables);
            _exitGameButton.OnClickAsObservable.Subscribe(_ => OnExitButton()).AddTo(ActivateDisposables);
        }

        private void OnReturn()
        {
            _manager.Hide(this);
            _manager.Show<BottomPanelWindow>();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Play);
        }

        public void SetLose(RewardContains rewardContains, bool isResurrect, Action claim, Action resurrect, bool isExitGame=false)
        {
            _isWin = false;
            _claim = claim;
            _resurrect = resurrect;
            if (isExitGame)
            {
                _resurrectBlock.SetActive(false);
                _winBlock.SetActive(false);
                _exitGameBlock.SetActive(true);
            }
            else
            {
                _exitGameBlock.SetActive(false);
                _resurrectBlock.SetActive(isResurrect);
                _winBlock.SetActive(!isResurrect);  
            }

            _title.text = LocalizationManager.GetTranslation(ScriptTerms.Messages.Defeat);
            
            _starsBlock.SetActive(false);
            _rewarded = true;
            rewardContains.Coin = (int) (rewardContains.Coin * 0.25f);
            rewardContains.Gem = 0;
            SetReward(_iapService.CanShowReward.Value);
            
            SaveStats(rewardContains);
        }
        
        public void SetWin(RewardContains rewardContains, int stars, Action claim, Action goOn,
            SceneInstances sceneInstances)
        {
            _isWin = true;
            _claim = claim;
            _continue = goOn;
            _exitGameBlock.SetActive(false);
            _resurrectBlock.SetActive(false);
            _winBlock.SetActive(true);
            sceneInstances.PlayerBuilder.DestroyStage();
            _title.text = LocalizationManager.GetTranslation(ScriptTerms.Windows_PlayResult.StageClear);
            
            foreach (var star in _stars)
            {
                star.SetActive(false);
            }
            
            for (int i = 0; i < stars; i++)
            {
                _stars[i].SetActive(true);
            }
            
            _starsBlock.SetActive(true);
           

            SaveStats(rewardContains);
            
        }

        private void SaveStats(RewardContains rewardContains)
        {
            _coinRewardCountBlock.SetValue(rewardContains.Coin);
            _gemRewardCountBlock.SetValue(rewardContains.Gem);
            _coinsCount = rewardContains.Coin;
            _gemCount = rewardContains.Gem;
        }

        private void OnClaim()
        {
            _claim?.Invoke();
            _dataCentralService.StatsDataModel.AddCoinsCount(_coinsCount);
            _dataCentralService.StatsDataModel.AddGemsCount(_gemCount);
            _dataCentralService.SaveFull();
            _continue?.Invoke();
            if (_isWin) _upgradeStageIndex?.Invoke();
        }

        private void OnRewardClaim()
        {
            _rewarded = true;
            _iapService.ShowRewardedBreak(RewardedClaim);
        }

        private void RewardedClaim(bool value)
        {
            if (value)
            {
                _coinsCount *= 2;
                _coinRewardCountBlock.SetValue(_coinsCount);
                OnClaim();
            }
        }

        private void SetReward(bool canShowReward)
        {
            _coinRewardCountBlock.SetBonusVisual(canShowReward && !_rewarded);
            _rewardClaimButton.gameObject.SetActive(canShowReward && !_rewarded); 
            _rewardResurrectButton.gameObject.SetActive(canShowReward); 
        }

        private void OnDefeatButton()
        {
            _resurrectBlock.SetActive(false);
            _winBlock.SetActive(true);
        }
        private void OnExitButton()
        {
            _exitGameBlock.SetActive(false);
            _winBlock.SetActive(true);
        }

        private void OnResurrect()
        {
            _iapService.ShowRewardedBreak(RewardResurrect);
        }

        private void RewardResurrect(bool value)
        {
            if (value)
            {
                _resurrect?.Invoke();
                _manager.Hide(this);
            }
        }
    }
}