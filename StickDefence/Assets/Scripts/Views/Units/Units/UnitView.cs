using System;
using Enums;
using Models.Attacking;
using Models.Move;
using UnityEngine;
using Views.Health;
using Views.Move;
using Views.Units.Animations;
using Random = UnityEngine.Random;

namespace Views.Units.Units
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private UnitFollowPath _unitFollowPath;
        [SerializeField] private Transform _body;
        [SerializeField] private Animator _bodyAnimator;
        [SerializeField] private Damageable _damageable;
        [SerializeField] private AttackBlockView _attackBlockView;
        [SerializeField] private Collider2D _unitCollider;
        [SerializeField] private float _timeToDestroy = 1f;
        [SerializeField] private UnitAnimationCallbacks _unitAnimationCallbacks;
        public UnitFollowPath UnitFollowPath => _unitFollowPath;
        public Animator BodyAnimator => _bodyAnimator;
        public Damageable Damageable => _damageable;
        public float TimerToDestroy => _timeToDestroy;
        public Collider2D UnitCollider => _unitCollider;
        public AttackBlockView AttackBlockView => _attackBlockView;
        public UnitAnimationCallbacks UnitAnimationCallbacks => _unitAnimationCallbacks;
        
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

        public void SetRandomBodyZ()
        {
            var vector3 = _body.localPosition;
            vector3.z = Random.Range(1f, 2f);
            _body.localPosition = vector3;
        }
        
        public void InitUnitMove(PathTypesEnum pathTypes, Transform[] pathElements)
        {
            _unitFollowPath.Init(pathTypes, pathElements);
        }
    }
}