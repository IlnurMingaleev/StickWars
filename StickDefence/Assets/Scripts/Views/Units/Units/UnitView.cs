using System;
using Enums;
using Models.Attacking;
using Models.Move;
using Spine.Unity;
using UI.Common;
using UniRx;
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
        [SerializeField] private UIBar _healthBar;
        [SerializeField] private MeshRenderer _meshRenderer;
        public UnitFollowPath UnitFollowPath => _unitFollowPath;
        public Animator BodyAnimator => _bodyAnimator;
        public Damageable Damageable => _damageable;
        public float TimerToDestroy => _timeToDestroy;
        public Collider2D UnitCollider => _unitCollider;
        public AttackBlockView AttackBlockView => _attackBlockView;
        public UnitAnimationCallbacks UnitAnimationCallbacks => _unitAnimationCallbacks;
        public UIBar HealthBar => _healthBar;
        public ReactiveProperty<float> Speed => _unitFollowPath.CurrentSpeed;

        private Action _enableAction;
        private Action _disableAction;
        private CompositeDisposable _disposable = new CompositeDisposable();
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

        public void SubscribeOnHealthChanged(UnitTypeEnum unitType)
        {
            _damageable.HealthCurrent
                .Subscribe(health => HealthBar.SetBarFiilAmount(health, _damageable.HealthMax.Value, unitType))
                .AddTo(_disposable);
        }

        public void SetSortingOrder(int pathCount,int pathIndex)
        {
            _meshRenderer.sortingOrder= pathCount - pathIndex;
            /*var vector3 = _body.position;
            vector3.z = 2 * pathIndex;
            _body.position = vector3;*/
        }
    }
}