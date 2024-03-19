using System;
using Anim.Battle.Fortress;
using Enums;
using Models.Attacking;
using Models.Merge;
using Models.Move;
using Models.Units;
using UniRx;
using UnityEngine;
using Views.Health;
using Views.Move;
using Views.Units.Animations;
using Random = UnityEngine.Random;

namespace Views.Units.Units
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private PlayerUnitTypeEnum _unitType;
        [SerializeField] private AttackBlockView _attackBlockView;
        [SerializeField] private Transform _body;
        [SerializeField] private Animator _bodyAnimator;
        [SerializeField] private float _timeToDestroy = 1f;
        [SerializeField] private UnitAnimationCallbacks _unitAnimationCallbacks;
        [SerializeField] private Damageable _damageable;
        private ReactiveProperty<bool> _isActive = new ReactiveProperty<bool>(false);
        public Animator BodyAnimator => _bodyAnimator;
        public Damageable Damageable => _damageable;
        public float TimerToDestroy => _timeToDestroy;
        public AttackBlockView AttackBlockView => _attackBlockView;
        public UnitAnimationCallbacks UnitAnimationCallbacks => _unitAnimationCallbacks;
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;
        public PlayerUnitTypeEnum UnitType => _unitType;
        
        private Action _enableAction;
        private Action _disableAction;

        public void InitUnityActions(Action enableAction, Action disableAction)
        {
            _enableAction = enableAction;
            _disableAction = disableAction;
        }

       

        private void OnEnable()
        {
            _enableAction?.Invoke();
        }

        private void OnDisable()
        {
            _disableAction?.Invoke();
        }
        
        
    }
}