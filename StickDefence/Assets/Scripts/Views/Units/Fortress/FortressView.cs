using System;
using Anim.Battle.Fortress;
using Models.Attacking;
using UniRx;
using UnityEngine;
using Views.Health;

namespace Views.Units.Fortress
{
    public class FortressView : MonoBehaviour
    {
        [SerializeField] private BattleFortressLaunch _battleFortressLaunch;
        [SerializeField] private Damageable _damageable;
        [SerializeField] private AttackBlockView _attackBlockView;

        private ReactiveProperty<bool> _isActive = new ReactiveProperty<bool>(false);
        
        public Damageable Damageable => _damageable;
        public ReactiveProperty<float> Speed  = new ReactiveProperty<float>(0);

        public IReadOnlyReactiveProperty<int> HealthCurrent => _damageable.HealthCurrent;
        public IReadOnlyReactiveProperty<int> HealthMax => _damageable.HealthMax;
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;
          
        public bool IsLaunchIsProgress => _battleFortressLaunch.IsLaunchIsProgress;
        
        private void OnEnable()
        {
            _isActive.Value = true;
            _damageable.SetSpeedToCalculatePredict(Speed);
        }

        private void OnDisable()
        {
            _isActive.Value = false;
        }
        
        public void StartPrepare()
        {
            _battleFortressLaunch.StartPrepare();
        }
        
        public void StartLaunchAnim()
        {
            _battleFortressLaunch.StartLaunchAnim();
        }

        public void RessurectHealth()
        {
            
        }
    }
}