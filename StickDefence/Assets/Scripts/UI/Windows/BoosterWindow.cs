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
using TonkoGames.StateMachine;
using UI.Common;
using UI.UIManager;
using UI.Windows;
using UnityEngine.UI;

namespace Ui.Windows
{
    public class BoosterWindow : Window
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private UIButton _skipBtn;
        [SerializeField] private UIButton _continueBtn;
        
        [SerializeField] private Image _attackSpeedImage;
        [SerializeField] private Image _gainCoinsImage;
        [SerializeField] private Image _autoMerge;
        [SerializeField] private RectTransform _skipBtnIcon; 
        private float fadeTime = 1f;
        
        [Inject] private ICoreStateMachine _coreStateMachine;
        [Inject] private IIAPService _iapService;
        [Inject] protected BoosterManager _boosterManager;
        [Inject] private ITimerService _timerService;

        private BoosterTypeEnum _lastBoosterTypeEnum; 
        private LocalizedString _currentLocalizedString;

        private Booster _selectableBooster;
        
        protected override void OnActivate()
        {
            base.OnActivate();
            //_continueBtn.OnClickAsObservable.Subscribe(_ => OnClickContinueBtn()).AddTo(_disposable);
            LocalizationManager.OnLocalizeEvent += UpdateLanguage;
            StartCoroutine(BlinkAnimation());
            Cursor.lockState = CursorLockMode.None;
            
            _skipBtn.OnClickAsObservable.Subscribe(_ => ShowRewardBooster()).AddTo(ActivateDisposables);
        }

        private void UpdateLanguage()
        {
            SetTitleLabel(_currentLocalizedString);
        }

        public void SetTitleLabel(LocalizedString titleText)
        {
            if(_currentLocalizedString != titleText)_currentLocalizedString = titleText;
            _titleLabel.text = titleText;
        }

        private void OnClickContinueBtn()
        {
           
            _manager.Hide(this);
            _manager.Show<TopPanelWindow>(WindowPriority.TopPanel);
           
            
        }

        private void OnClickMainMenuBtn()
        {
            
            _manager.Hide(this);
        }

        public void Subscribe(Booster booster)
        {
            _selectableBooster = booster;
        }

        private void ShowRewardBooster()
        {
            if (_selectableBooster == null)
                return;
            
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
                OnRewardClaim(_selectableBooster);
            }

            _selectableBooster = null;
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
        
        private void OnRewardClaim(Booster booster)
        {

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
            //_lastBoosterTypeEnum = BoosterTypeEnum.PogoStick;

        }

        public void ActivateAutoMergeImage()
        {
            DeactivateAllImages();
            _autoMerge.gameObject.SetActive(true);
            //_lastBoosterTypeEnum = BoosterTypeEnum.Boots;
        }

        public void ActivateCoinsImage()
        {
            DeactivateAllImages();
            _gainCoinsImage.gameObject.SetActive(true);
           // _lastBoosterTypeEnum = BoosterTypeEnum.Shield;
        }
        
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            LocalizationManager.OnLocalizeEvent -= UpdateLanguage;
            
        }
    }
    
}