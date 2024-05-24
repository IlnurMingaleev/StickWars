using Models.Player;
using Models.Timers;
using TonkoGames.Sound;
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
        [SerializeField] protected int _enemyLayer;
        [Inject] protected IPlayer _player;
        [Inject] protected ITimerService _timerService;
        [Inject] protected IWindowManager _windowManager;
        [Inject] protected ICoreStateMachine _coreStateMachine;
        [Inject] protected ISoundManager _soundManager;
        protected ITimerModel _timerModel;
        protected float _explosionRadius;
        protected bool _skillCooldownPassed = true; 
        protected float _cooldownTime = 30f;
        protected int _skillDamage = 7000;
        protected int _enemyLayerMask;
        protected BottomPanelWindow _bottomPanelWindow;
        protected CompositeDisposable _disposable = new CompositeDisposable();
        

        public bool SkillCooldownPassed => _skillCooldownPassed;
        public Transform AimView
        {
            get { return _aimView; }
            set { _aimView = value; }
        }

        public abstract void LaunchMissile(Vector3 mousePosition);

        protected void Start()
        {
            _enemyLayer = LayerMask.NameToLayer("Enemy");
            _enemyLayerMask = (1 << _enemyLayer);
            _explosionRadius = 4f;
            _disposable.Clear();
            _coreStateMachine.RunTimeStateMachine.RunTimeState.Subscribe(_ => OnRunTimeStateSwitch(_))
                .AddTo(_disposable);
        }


        protected void StartTimer()
        {
            _skillCooldownPassed = false;
            DeacivateSkillButton();
            _skillTimer.InitAndStart((int) _cooldownTime, () =>
            {
                ActivateSkillButton();
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
           _skillCooldownPassed = true;
           _skillTimer.FinishTimer();
           _disposable.Clear();
        }

        protected abstract void ActivateSkillButton();

        protected abstract void DeacivateSkillButton();
    }
}