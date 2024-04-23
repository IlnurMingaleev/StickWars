using System;
using System.Collections.Generic;
using Models.Move;
using TMPro;
using UniRx;
using UnityEngine;

namespace Views.Move
{
    public class UnitFollowPath : MonoBehaviour
    {
        [SerializeField] private MovementTypeEnum _movementType = MovementTypeEnum.Move;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _maxDistance = .1f;
        [SerializeField] private int _movementDirection = 1;
        [SerializeField] private Transform _scalableBodyX;
        [SerializeField] private bool _isBodyLookLeft = false;
        
        private UnitMovementPath _unitMovementPath;

        private IEnumerator<Transform> _pointInPath;

        private bool _boolEndOneWay = false;
        private float _lastPosX;

        private ReactiveProperty<bool> _isScaleBodyRight = new ReactiveProperty<bool>(false);

        public ReactiveProperty<float> CurrentSpeed { get; private set; } = new ReactiveProperty<float>(0);
        //public IReadOnlyReactiveProperty<float> CurrentSpeed => CurrentSpeed;

        private void OnEnable()
        {
            _isScaleBodyRight.SkipLatestValueOnSubscribe().TakeUntilDisable(this).Subscribe(OnScale);
            _lastPosX = transform.position.x;
        }

        public void Init(PathTypesEnum pathTypes, Transform[] pathElements)
        {
            _unitMovementPath = gameObject.AddComponent<UnitMovementPath>();
            _unitMovementPath.Init(pathTypes, pathElements, _movementDirection, EndOneWay);
            
            _pointInPath = _unitMovementPath.GetNextPathPoint();
            _pointInPath.MoveNext();
            CurrentSpeed.Value = _speed;
        }

        private void OnScale(bool value)
        {
            var scale = _scalableBodyX.localScale;
            
            if (value)
            {
                scale.x = _isBodyLookLeft ? -1 : 1;
            }
            else
            {
                scale.x = _isBodyLookLeft ? 1 : -1;
            }
            
            _scalableBodyX.localScale = scale;
        }
        
        private void EndOneWay()
        {
            CurrentSpeed.Value = 0;
            _boolEndOneWay = true;
        }

        public void RestartOnWay()
        {
            _boolEndOneWay = false;
        }
        
        public void Move()
        {
            if (_boolEndOneWay)
                return;

            var selfPosition = transform.position;
            var pointPosition = _pointInPath.Current.position;
            
            if (_movementType == MovementTypeEnum.Move)
            {
                selfPosition = Vector2.MoveTowards(selfPosition, pointPosition, Time.deltaTime * _speed);
            }else if (_movementType == MovementTypeEnum.Lerp) 
            {
                selfPosition = Vector2.Lerp(selfPosition, pointPosition, Time.deltaTime * _speed);
            }

            transform.position = selfPosition;
            
            var distanceSqr = ((Vector2) selfPosition - (Vector2) pointPosition).sqrMagnitude;

            if (distanceSqr < _maxDistance * _maxDistance)
            {
                _pointInPath.MoveNext();
                CurrentSpeed.Value = _speed;
            }
            else
            {
                CurrentSpeed.Value = 0;
            }

            _isScaleBodyRight.Value = _lastPosX <= selfPosition.x;
            _lastPosX = selfPosition.x;
        }
    }
}