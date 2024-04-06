using System.Collections;
using Models.Player;
using Models.Timers;
using UI.UIManager;
using UI.Windows;
using UnityEngine;
using UnityEngine.WSA;
using VContainer;
using Views.Health;
using Views.Units.Units;

namespace Models.Controllers.Skills
{
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem _particleSystem;
        [SerializeField] protected Transform _projectileView;
        [SerializeField] protected Transform _aimView;
        [Inject] protected IPlayer _player;
        [Inject] protected ITimerService _timerService;
        [Inject] protected IWindowManager _windowManager;
        protected ITimerModel _timerModel;
        protected float _explosionRadius;
        protected bool _skillCooldownPassed = true;
        [SerializeField] protected float _cooldownTime;
        protected BottomPanelWindow _bottomPanelWindow;

        public Transform AimView
        {
            get { return _aimView; }
            set { _aimView = value; }
        }

        public abstract void LaunchMissile(Vector3 mousePosition);

        public void StartTimer()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
            _timerModel = _timerService.AddGameTimer(_cooldownTime,
                f => { UpdateUIBar((float)(1.0f - f/_cooldownTime) ); },
                () => { _skillCooldownPassed = true; });
        }

        protected abstract void UpdateUIBar(float value);
    }
}