using System;
using Anim.Battle.Fortress;
using Models.Attacking;
using Models.Battle;
using Models.Fortress;
using UniRx;
using UnityEngine;
using Views.Health;
using Views.Units.Animations;

namespace Views.Units.Fortress
{
    public class PlayerViewTwo : MonoBehaviour
    {
       // [SerializeField] private BattleFortressLaunch _battleFortressLaunch;
        [SerializeField] private Damageable _damageable;
        [SerializeField] private AttackBlockView _attackBlockView;
        [SerializeField] private Animator _animator;
        [SerializeField] private UnitAnimationCallbacks _unitAnimationCallbacks;

        private ReactiveProperty<bool> _isActive = new ReactiveProperty<bool>(false);
        public Damageable Damageable => _damageable;
        public AttackBlockView AttackBlockView => _attackBlockView;
        public Animator Animator => _animator;
        public IReadOnlyReactiveProperty<int> HealthCurrent => _damageable.HealthCurrent;
        public IReadOnlyReactiveProperty<int> HealthMax => _damageable.HealthMax;
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;
        public UnitAnimationCallbacks UnitAnimationCallbacks => _unitAnimationCallbacks;

        private PlayerUnitsBuilderTwo _playerUnitsBuilder;
        private PlayerUnitModel _playerUnitModel;
        private void OnEnable()
        {
            _isActive.Value = true;
        }

        private void OnDisable()
        {
            _isActive.Value = false;
        }
        
        
        public void StartLaunchAnim()
        {
           
        }
        
    }
}