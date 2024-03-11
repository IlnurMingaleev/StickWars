using Abu;
using TonkoGames.StateMachine;
using Enums;
using TonkoGames.StateMachine.Enums;
using UI.Common;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;
    
namespace UI.Content.Tutor
{
    public class UITutorialDefaultFade: UIBehaviour
    {
        [SerializeField] private TutorialStepsEnum _tutorialStateEnum;
        [SerializeField] private TutorialLineEnum _tutorialLineEnum;
        [SerializeField] private UIButton _buttonNextState;

        [Header("Hand")]
        [SerializeField] private RectTransform _handPrefab;
        [SerializeField] private RectTransform _targetHandRect;
        [SerializeField] private Vector2 _handOffset;

        private RectTransform _handVariant;

        private bool _isShown = false;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly IWindowManager _windowManager;
        private TutorialHighlight _tutorialHighlight;
        
        private IReadOnlyReactiveProperty<TutorialStepsEnum> _tutorialStep;
        private CompositeDisposable _disposables = new CompositeDisposable();

        protected override void Awake()
        {
            base.Awake();
            _tutorialStep = _coreStateMachine.TutorialStateMachine.GetTutorialStepReactive(_tutorialLineEnum);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!_coreStateMachine.TutorialStateMachine.IsTutorialShown(_tutorialLineEnum))
            {
                _tutorialStep.Subscribe(CheckState).AddTo(_disposables);
            }
            else
            {
                _isShown = false;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Clear();
            _disposables.Clear();
        }

        private void CheckState(TutorialStepsEnum tutorialStateEnum)
        {
            Clear();
            if (tutorialStateEnum == _tutorialStateEnum)
            {
                _tutorialHighlight = (TutorialHighlight)gameObject.AddComponent(typeof(TutorialHighlight));
                _tutorialHighlight.SetTutorialFade(_windowManager.FindWindow<TutorialWindow>().TutorialFadeImage);
                _isShown = true;
                if (_buttonNextState != null)
                {
                    _buttonNextState.OnClickAsObservable.Subscribe(_ => ButtonNextState()).AddTo(_disposables);
                }
                if (_handPrefab != null)
                {
                    _handVariant = Instantiate(_handPrefab, transform);
                    var handVariantPosition = _handVariant.position;
                    handVariantPosition.x += _handOffset.x;
                    handVariantPosition.y += _handOffset.y;
                    _handVariant.position = handVariantPosition;
                }
            }
        }

        private void Clear()
        {
            if (_isShown)
            {
                Destroy(_tutorialHighlight);
                _isShown = false;
                _disposables.Clear();
            }
            
            if (_handVariant != null)
            {
                Destroy(_handVariant.gameObject);
            }
        }

        private void ButtonNextState()
        {
            _coreStateMachine.TutorialStateMachine.NextStateBuyType(_tutorialLineEnum);
        }

        private void SetHandOffset()
        {
            
        }
    }
}