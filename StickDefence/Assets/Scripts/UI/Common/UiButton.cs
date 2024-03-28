using System;
using System.Collections.Generic;
using System.Linq;
using TonkoGames.Sound;
using Cysharp.Threading.Tasks;
using I2.Loc;
using TMPro;
using UI.Animations;
using UI.Constants;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace UI.Common
{
 [RequireComponent(typeof(Button))]
    public class UIButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const int LongPressTime = 3;
        private const int LabelPressOffset = -6;

        [Inject] private readonly ISoundManager _soundManager;

        #region Inspector
#pragma warning disable 0649
        [SerializeField] private Image _targetInteractableImage;
        [SerializeField] private Color _colorActive;
        [SerializeField] private Color _colorInActive;
        [SerializeField] protected TextMeshProUGUI _label;
        [SerializeField] protected Localize _localizedLabel;
        [SerializeField] protected UIRewardParticles _particles;
        [SerializeField] protected UIButtonClickedEvent _onClick = new UIButtonClickedEvent();
        [SerializeField] protected UIButtonDownedEvent  _onDown  = new UIButtonDownedEvent();
        [SerializeField] protected UIButtonUpedEvent    _onUp    = new UIButtonUpedEvent();
        [SerializeField] protected UIButtonLongPressedEvent  _onLongPress  = new UIButtonLongPressedEvent();
#pragma warning restore 0649
        #endregion

        #region private fields
        protected Animator _animator;
        protected Button _button;
        protected CanvasGroup _group;

        private TapScaleAnimation _tapScaleAnimation;
        private bool _clicked;
        private bool _tapScaleCompleted;

        private bool _isDown;
        #endregion

        #region properties
        protected bool IsDown
        {
            get => _isDown;
            set
            {
                if (_isDown != value)
                {
                    _isDown = value;

                    if (_isDown) DoDown();
                    else DoUp();
                }
            }
        }

        public Animator Animator => _animator ? _animator : _animator = GetComponent<Animator>();

        public Button Button => _button ? _button : _button = GetComponent<Button>();

        public CanvasGroup Group => _group ? _group : _group = GetComponent<CanvasGroup>();

        public TextMeshProUGUI Label => _label;
        public Localize LocalizedLabel => _localizedLabel;

        public UIButtonClickedEvent OnClick => _onClick;

        public IObservable<UIButton> OnClickAsObservable => OnClick.AsObservable();

        public UIButtonDownedEvent OnDown => _onDown;

        public UIButtonUpedEvent OnUp => _onUp;

        public UIButtonLongPressedEvent OnLongPressed => _onLongPress;

        public ReactiveProperty<bool> IsPressed { get; protected set; } = new ReactiveProperty<bool>(false);

        public bool IsInteractable
        {
            get => _group ? _group.interactable : Button.IsInteractable();
            set
            {
                Button.interactable = value;
                if (_targetInteractableImage != null )
                {
                    _targetInteractableImage.color = !value ? _colorInActive : _colorActive;
                }
         
                if (Group != null)
                {
                    Group.interactable = Group.blocksRaycasts = value;
                    Group.alpha = value ? 1f : DefaultUIConstants.InactiveButtonAlpha;
                }
                if (_particles != null)
                {
                    _particles.gameObject.SetActive(value);
                }
            }
        }
        #endregion

        #region MonoBehaviour

        protected override void Awake()
        {
            base.Awake();

            _tapScaleAnimation = GetComponent<TapScaleAnimation>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();


            Button.onClick.AddListener(OnButtonClicked);
            if (_tapScaleAnimation != null)
                _tapScaleAnimation.AnimationEvent += OnTapScaleAnimationCompleted;
        }

        private void OnTapScaleAnimationCompleted()
        {
            _tapScaleCompleted = true;
            if (_clicked)
                InvokeOnClick();
        }

        protected override void OnDisable()
        {
            Button.onClick.RemoveListener(OnButtonClicked);

            base.OnDisable();

            IsPressed.Value = false;
            UpdateIsDownState();
            if (_tapScaleAnimation != null)
                _tapScaleAnimation.AnimationEvent -= OnTapScaleAnimationCompleted;
        }

        protected virtual void LateUpdate()
        {
            UpdateIsDownState();
        }
        #endregion

        #region private methods
        private readonly Vector2 OffsetVector = new Vector2(0, LabelPressOffset);

        protected virtual void DoUp()
        {
            OnUp?.Invoke(this);
            if (Label != null)
                Label.rectTransform.anchoredPosition -= OffsetVector;
        }

        protected virtual void DoDown()
        {
            OnDown?.Invoke(this);
            if (Label != null)
                Label.rectTransform.anchoredPosition += OffsetVector;
        }

        private float _pressedTime = 0;
        protected void UpdateIsDownState()
        {
            IsDown = GetIsDownState();
            UpdateIsLongPressedState();
        }

        protected async void UpdateIsLongPressedState()
        {
            if (IsDown)
            {
                var oldPressedTime = _pressedTime;
                _pressedTime += Time.unscaledDeltaTime;
                if (oldPressedTime < LongPressTime && _pressedTime >= LongPressTime)
                {
                    await UniTask.Yield(PlayerLoopTiming.PreUpdate);
                    if (_pressedTime >= LongPressTime)
                        OnLongPressed?.Invoke(this);
                }
            }
            else
                _pressedTime = 0;
        }

        protected bool GetIsDownState()
        {
            return IsActive() && Button.IsInteractable() && IsPressed.Value;
        }
        #endregion

        #region delegates
        protected virtual void OnButtonClicked()
        {
            _clicked = true;
            if (_tapScaleAnimation == null || _tapScaleCompleted)
                InvokeOnClick();
        }

        private void InvokeOnClick()
        {
            _tapScaleCompleted = false;
            _clicked = false;

//            if (OnClick.Listeners > 0) _soundManager.PlayUiButtonClick();
            OnClick.Invoke(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            //UnityEngine.Debug.LogWarning($"{this.name} OnPointerDown\r\n{eventData.ToString()}");

            IsPressed.Value = true;
/*
            var pointerId = eventData.pointerId;

            this.ObserveEveryValueChanged(button => pointerId < 0 && Input.GetMouseButton(Math.Abs(pointerId) - 1)
                                                    || Input.touches.Any(touch=> touch.fingerId == pointerId && touch
                                                    .phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled))
                .TakeUntilDisable(this)
                .TakeWhile(b => IsPressed.Value)
                .FirstOrDefault(press => !press)
                .Subscribe(press =>
                {
                    //UnityEngine.Debug.LogWarning($"{this.name} TouchUp\r\n{eventData.ToString()}");
                    IsPressed.Value = false;
                    UpdateIsDownState();
                });
*/
            UpdateIsDownState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //UnityEngine.Debug.LogWarning($"{this.name} OnPointerUp\r\n{eventData.ToString()}");
            IsPressed.Value = false;

            UpdateIsDownState();
        }
        #endregion
    }

    public static class UIButtonExtensions
    {
        public static IObservable<UIButton> AsObservable(this UIButtonEvent uibEvent)
        {
            return Observable.FromEvent<UnityAction<UIButton>, UIButton>(h => new UnityAction<UIButton>(h),
                uibEvent.AddListener, uibEvent.RemoveListener);
        }
    }

    public class UIButtonEvent : UnityEvent<UIButton>
    {
        public int Listeners => _listeners.Count;
        private HashSet<UnityAction<UIButton>> _listeners = new HashSet<UnityAction<UIButton>>();

        public new void AddListener(UnityAction<UIButton> action)
        {
            base.AddListener(action);
            _listeners.Add(action);
        }

        public new void RemoveListener(UnityAction<UIButton> action)
        {
            base.RemoveListener(action);
            _listeners.Remove(action);
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();
            _listeners.Clear();
        }

        public List<UnityAction<UIButton>> GetListenersAndClear()
        {
            var result = _listeners.ToList();
            RemoveAllListeners();
            return result;
        }

        //public
    }

    [Serializable]
    public class UIButtonClickedEvent : UIButtonEvent { }

    [Serializable]
    public class UIButtonDownedEvent : UIButtonEvent { }

    [Serializable]
    public class UIButtonUpedEvent : UIButtonEvent { }

    [Serializable]
    public class UIButtonLongPressedEvent : UIButtonEvent { }
}