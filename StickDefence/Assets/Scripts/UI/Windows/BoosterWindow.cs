using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;
using DG.Tweening;
using Enums;
using I2.Loc;
using Models.Battle.Boosters;
using Models.IAP;
using Models.Timers;
using Sirenix.OdinInspector.Editor.Drawers;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Ui.Common;
using UI.Common;
using UI.UIManager;
using UI.Windows;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Ui.Windows
{
    public class BoosterWindow : Window
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private UIButton _skipBtn;
        [SerializeField] private UIButton _continueBtn;
        [SerializeField] private UIButton _closeBtn;
        [SerializeField] private Image _attackSpeedImage;
        [SerializeField] private Image _gainCoinsImage;
        [SerializeField] private Image _autoMerge;
        [SerializeField] private RectTransform _skipBtnIcon; 
        private float fadeTime = 1f;
        
        [Inject] private ICoreStateMachine _coreStateMachine;
        [Inject] private IIAPService _iapService;
        [Inject] private ITimerService _timerService;

        private BoosterTypeEnum _lastBoosterTypeEnum;
        private BoosterManager _boosterManager;
        private WindowPriority Priority = WindowPriority.TopPanel;
        
        protected override void OnActivate()
        {
            base.OnActivate();
            //StartCoroutine(BlinkAnimation());
            Cursor.lockState = CursorLockMode.None;
            
            _skipBtn.OnClickAsObservable.Subscribe(_ => ShowRewardBooster()).AddTo(ActivateDisposables);
            _closeBtn.OnClickAsObservable.Subscribe(_ => CloseWindow()).AddTo(ActivateDisposables);
            
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Pause);
        }

        private void CloseWindow()
        {
            _manager.Hide(this);
        }

        public void Init(BoosterManager boosterManager)
        {
            _boosterManager = boosterManager;
            SelectRandomBooster();
        }
        
        
        private void ShowRewardBooster()
        {
            _iapService.RewardedBreakComplete += ShownRewardBooster;
            Debug.Log("asdf _skipBtn");
            _iapService.ShowRewardedBreak();
        }

        private void ShownRewardBooster(bool value)
        {
            Debug.Log("asdf _skipBtn " + value);
            _iapService.RewardedBreakComplete -= ShownRewardBooster;
            
            if (value)
            {
                OnRewardClaim();
            }
            
        }

        private IEnumerator BlinkAnimation()
        {
            while (true)
            {
                _skipBtnIcon.DOScale(0.75f, fadeTime).SetEase(Ease.Linear);
                yield return new WaitForSeconds(fadeTime);
                _skipBtnIcon.DOScale(0.7f, fadeTime).SetEase(Ease.Linear);
                yield return new WaitForSeconds(fadeTime);
            }
        }
        
        private void OnRewardClaim()
        {
            _boosterManager.ApplyBooster(_lastBoosterTypeEnum);
        }
     
        public void DeactivateAllImages()
        {
            _attackSpeedImage.gameObject.SetActive(false);
            _autoMerge.gameObject.SetActive(false);
            _gainCoinsImage.gameObject.SetActive(false);
          
        }

        public void ActivateAttackSpeedImage()
        {
            DeactivateAllImages();
            _attackSpeedImage.gameObject.SetActive(true);
            _titleLabel.text = ScriptLocalization.Messages.AttackSpeed;
        }

        public void ActivateAutoMergeImage()
        {
            DeactivateAllImages();
            _autoMerge.gameObject.SetActive(true);
            _titleLabel.text = ScriptLocalization.Messages.AutoMerge;
        }

        public void ActivateCoinsImage()
        {
            DeactivateAllImages();
            _gainCoinsImage.gameObject.SetActive(true);
            _titleLabel.text = ScriptLocalization.Messages.GainCoins;
        }

        public void SelectRandomBooster()
        {
           // int randomInt = Random.Range(1, 3);
            _lastBoosterTypeEnum = BoosterTypeEnum.AutoMerge;
            switch (_lastBoosterTypeEnum)
            {
                case BoosterTypeEnum.AttackSpeed:
                    ActivateAttackSpeedImage();
                    break;
                case BoosterTypeEnum.AutoMerge:
                    ActivateAutoMergeImage();
                    break;
                case BoosterTypeEnum.GainCoins:
                    ActivateCoinsImage();
                    break;
            }
           
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Play);
        }
    }
    
}