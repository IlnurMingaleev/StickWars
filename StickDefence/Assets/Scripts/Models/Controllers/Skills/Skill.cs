using Models.Player;
using Models.Timers;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;


namespace Models.Controllers.Skills
{
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem _particleSystem;
        [SerializeField] protected Transform _projectileView;
        [SerializeField] protected Transform _aimView;
        [SerializeField] protected CoroutineTimer _skillTimer;
        [Inject] protected IPlayer _player;
        [Inject] protected ITimerService _timerService;
        [Inject] protected IWindowManager _windowManager;
        [Inject] protected ICoreStateMachine _coreStateMachine;
        protected ITimerModel _timerModel;
        protected float _explosionRadius;
        protected bool _skillCooldownPassed = true; 
        protected float _cooldownTime = 30000f;
        protected BottomPanelWindow _bottomPanelWindow;
        protected CompositeDisposable _disposable = new CompositeDisposable();
        public bool SkillCooldownPassed => _skillCooldownPassed;
        public Transform AimView
        {
            get { return _aimView; }
            set { _aimView = value; }
        }

        public abstract void LaunchMissile(Vector3 mousePosition);

        private void Start()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
            _coreStateMachine.RunTimeStateMachine.RunTimeState.Subscribe(_ => OnRunTimeStateSwitch(_))
                .AddTo(_disposable);
        }


        protected void StartTimer()
        {
            _skillCooldownPassed = false;
            _skillTimer.InitAndStart((int) _cooldownTime, () =>
            {
                _skillCooldownPassed = true;
                _skillTimer.FinishTimer();
            }, f => { UpdateUIBar((float) (1.0f - f / _cooldownTime)); });

        }

        protected abstract void UpdateUIBar(float value);

        private void OnRunTimeStateSwitch(RunTimeStateEnum runTimeStateEnum)
        {
            switch (runTimeStateEnum)
            {
                case RunTimeStateEnum.Pause:
                    OnPause();
                    break;
                case RunTimeStateEnum.Play:
                    OnPlay();
                    break;
            }
        }

        private void OnPlay()
        {
           _skillTimer.StartTick();
        }

        private void OnPause()
        {
            _skillTimer.Pause();
        }

        
        private void OnDisable()
        {
           // _timerModel.StopTick();
           _skillTimer.FinishTimer();
           _disposable.Clear();
        }
    }
}