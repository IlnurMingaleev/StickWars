using System;
using System.Collections.Generic;
using TonkoGames.Analytics;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TonkoGames.StateMachine.Enums;
using Helpers;
using Models.IAP;
using Models.Player;
using Models.SO.Iaps;
using TMPro;
using Tools.GameTools;
using UI.Common;
using UI.Content.Rewards;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace UI.Content.Spin
{
    public class LuckySpinWindow :Window
    {
        [SerializeField] private UIButton _exitButton;
        [SerializeField] private List<SpinSlot> _spinSlots;
        [SerializeField] private UIButton _spin;
        [SerializeField] private Transform _spinBlock;
        [SerializeField] private TMP_Text _cooldownTimerLabel;
        [SerializeField] private UIButton _rewardSpinButton;

        private CompositeDisposable _disposable = new CompositeDisposable();

        private Tween _tween = null;

        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly IWindowManager _windowManager;
        [Inject] private readonly IPlayer _player;
        [Inject] private readonly IIAPService _iapService;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        
        private WindowPriority Priority = WindowPriority.AboveTopPanel;
        
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < _spinSlots.Count; i++)
            {
                _spinSlots[i].Init(_configManager.IapSO.LuckySpin[i]);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _exitButton.OnClickAsObservable.Subscribe(_ => ClosePopup()).AddTo(_disposable);
            _spin.OnClickAsObservable.Subscribe(_ => StartSpin()).AddTo(_disposable);
            _rewardSpinButton.OnClickAsObservable.Subscribe(_ => RewardSpinButton()).AddTo(_disposable);
            
            _player.DailyModel.SpinTimer.Subscribe(SetTimeCooldown).AddTo(_disposable);
            _player.DailyModel.CanSpin.Subscribe(value =>
            {
                _spin.IsInteractable = value;
                _rewardSpinButton.gameObject.SetActive(!value);
            }).AddTo(_disposable);

            _iapService.CanShowReward.Subscribe(value => _rewardSpinButton.IsInteractable = value).AddTo(_disposable);
            
            if (!_coreStateMachine.TutorialStateMachine.IsLuckySpinTutorialShown && _coreStateMachine.TutorialStateMachine.LuckySpinTutorialStep.Value == TutorialStepsEnum.NoneStart) 
            {
                //   _coreStateMachine.TutorialStateMachine.SetLuckySpinTutorialState(TutorialStepsEnum.LuckySpinDialog);
            }
        }

        
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _disposable.Clear();

            if (_tween != null)
            {
                _tween.Kill();
                ShowReward(CheckSlot(Random.Range(0, 360)));
                _tween = null;
            }
        }

        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        #region Tweens

        private void StartSpin()
        {
            GameAnalytics.Instance.PushEvent(StringsHelper.Analytics.spin, StringsHelper.Analytics.start_spin);
            _spin.IsInteractable = false;
            _tween = _spinBlock.DOLocalRotate(new Vector3(0, 0, -360), 115, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InBack)
                .SetSpeedBased(true)
                .OnComplete(EndStartTween);
        }
        
        private void EndStartTween()
        {
            _tween = _spinBlock.DOLocalRotate(new Vector3(0, 0, -360), 450, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetSpeedBased(true)
                .SetLoops(2, LoopType.Incremental)
                .OnComplete(EndEndlessTween);
        }
        
        private void EndEndlessTween()
        {
            _tween = _spinBlock.DOLocalRotate(new Vector3(0, 0, -Random.Range(0, 360)), 450, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetSpeedBased(true)
                .SetLoops(1, LoopType.Incremental)
                .OnComplete(EndEndlessRandomTween);
        }
        
        private void EndEndlessRandomTween()
        {
            _tween = _spinBlock.DOLocalRotate(new Vector3(0, 0, -360), 115, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutBack)
                .SetSpeedBased(true)
                .OnComplete(EndTweens);
        }
        

        #endregion
        
        private void EndTweens()
        {
            _tween = null;
            ShowReward(CheckSlot(_spinBlock.localRotation.eulerAngles.z));
        }

        private void ShowReward(RewardConfig rewardConfig)
        {
            _windowManager.Show<RewardCollectWindow>().Collect(new RewardContains()
            {
                Coin = rewardConfig.CoinReward,
                Gem = rewardConfig.GemReward,
                AttackSpeed = rewardConfig.AttackSpeedReward,
                AutoMerge = rewardConfig.AutoMergeReward,
                GainCoins = rewardConfig.GainCoinsReward,
            }).Forget();
            _player.DailyModel.UpdateSpin(rewardConfig);
            _rewardSpinButton.gameObject.SetActive(!_player.DailyModel.CanSpin.Value);
            _spin.IsInteractable = _player.DailyModel.CanSpin.Value;
        }
        private RewardConfig CheckSlot(float rotation)
        {
            float delta = 360f / _spinSlots.Count;
            rotation += delta / 2;
            
            int index = (int)((rotation - (rotation % delta)) / delta);

            if (index == _spinSlots.Count)
                index = 0;
            
            return _spinSlots[index].Config;
        }

        private void SetTimeCooldown(int second)
        {
            _cooldownTimerLabel.text = TextFormatHelper.GetTimeHhMmSsWithColons(TimeSpan.FromSeconds(second));
        }

        private void RewardSpinButton()
        {
            GameAnalytics.Instance.PushEvent(StringsHelper.Analytics.click_reward, StringsHelper.Analytics.spin);
            _iapService.ShowRewardedBreak(RewardSpinBreakComplete);
        }
        
        private void RewardSpinBreakComplete(bool value)
        {
            if (value)
            {
                _player.DailyModel.RewardSpinShown();
                StartSpin();
                _rewardSpinButton.gameObject.SetActive(false);
            }
            else
                _rewardSpinButton.IsInteractable = false;
        }
    }
}