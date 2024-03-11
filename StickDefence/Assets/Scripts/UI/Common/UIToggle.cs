using TonkoGames.Sound;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Common
{
    public class UIToggle : Toggle
    {
        [SerializeField] protected UINotificationCounter _notification;

        [Inject] private readonly ISoundManager _soundManager;

        private bool _muteSound = false;
        private bool _wasStart;
        
        private static readonly int _isOnParameterHash = Animator.StringToHash("IsOn");
        
        public bool MuteSound
        {
            set => _muteSound = value;
        }

        public BoolReactiveProperty IsOn;

        public UINotificationCounter Notification => _notification;

        protected override void Awake()
        {
            base.Awake();
            
            IsOn = new BoolReactiveProperty(isOn).AddTo(this);
            onValueChanged.AsObservable().Subscribe(x =>
            {
                IsOn.Value = x;
                if (animator)
                    animator.SetBool(_isOnParameterHash, x);
            }).AddTo(this);
            IsOn.Subscribe(x => isOn = x).AddTo(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.onValueChanged.AddListener(_OnValueChanged);

            if (_wasStart & animator)
            {
                _InitAnimator();
            }
        }

        protected override void OnDisable()
        {
            this.onValueChanged.RemoveListener(_OnValueChanged);
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();

            if (animator)
                _InitAnimator();
            
            _wasStart = true;
        }

        private void _InitAnimator()
        {
            animator.SetBool(_isOnParameterHash, isOn);
            animator.Update(0f);
        }
        
        private void _OnValueChanged(bool val)
        {
            if (Application.isPlaying && !_muteSound)
                _soundManager.PlayUiButtonClick();
        }
    }
}