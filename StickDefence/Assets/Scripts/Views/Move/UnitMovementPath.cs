using System;
using System.Collections.Generic;
using Models.Move;
using UnityEngine;

namespace Views.Move
{
    public class UnitMovementPath : MonoBehaviour
    {
        private int _movementDirection = 1;
        private int _moveTo = 0;
        private Transform[] _pathElements;
        private PathTypesEnum _pathTypes;
        private Action EndOneWay;

        public void Init(PathTypesEnum pathTypes, Transform[] pathElements, int movementDirection, Action endOneWay)
        {
            _pathTypes = pathTypes;
            _pathElements = pathElements;
            _movementDirection = movementDirection;
            EndOneWay = endOneWay;
        }

        public IEnumerator<Transform> GetNextPathPoint()
        {
            if (_pathElements == null || _pathElements.Length < 1)
            {
                yield break;
            }

            while (true)
            {
                yield return _pathElements[_moveTo];
                if (_pathElements.Length == 1)
                {
                    continue;
                }

                if (_pathTypes == PathTypesEnum.OneWay)
                {
                    if (_moveTo >= _pathElements.Length - 1)
                    {
                        EndOneWay?.Invoke();
                        break;
                    }
                }
                if (_pathTypes == PathTypesEnum.Linear)
                {
                    if (_moveTo <= 0)
                    {
                        _movementDirection = 1;
                    }else if (_moveTo >= _pathElements.Length - 1)
                    {
                        _movementDirection = -1;
                    }
                }

                _moveTo += _movementDirection;

                if (_pathTypes == PathTypesEnum.Loop)
                {
                    if (_moveTo >= _pathElements.Length)
                    {
                        _moveTo = 0;
                    }

                    if (_moveTo < 0)
                    {
                        _moveTo = _pathElements.Length - 1;
                    }
                }
            }
        }
    }
}