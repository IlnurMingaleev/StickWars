using System;
using UniRx;
using UnityEngine;

namespace Tools.GameTools
{
    public class ScaleCameraSize2D : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _sizeCamera;
        private const float DefaultWidth = 2400f;
        private const float DefaultHeight = 1600f;
        private const float MaxDelta = 1f;
        
        private float _defOrthographicSize = 0;
        private float _lastDelta = 1;
        
        private float Width => Screen.width;
        private float Height => Screen.height;
        private float NewDifference => Width / Height;
        private float DefaultDifference => DefaultWidth / DefaultHeight;
        
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private SnapCamera _snapCamera = SnapCamera.Center;
        
        private void Awake()
        {
            if (_camera != null) _camera.orthographicSize = _sizeCamera;
            _defOrthographicSize = _camera.orthographicSize;
        }

        private void OnDisable()
        {
            StopSnap();
        }

        public void StartSnap(SnapCamera snapCamera)
        {
            StopSnap();
            if (snapCamera == SnapCamera.Center)
                _camera.transform.localPosition = Vector3.zero;
            _snapCamera = snapCamera;
#if UNITY_WEBGL
            Observable.EveryFixedUpdate().Subscribe(_ => CheckDelta()).AddTo(_compositeDisposable);
#else
            CheckDelta();
#endif
        }

        public void StopSnap()
        {
            _compositeDisposable.Clear();
        }

        private void CheckDelta()
        {
            float tmpDelta = DefaultDifference / NewDifference;
            
            if (tmpDelta < 1f - MaxDelta)
            {
                tmpDelta = 1f - MaxDelta;
            }
            else if(tmpDelta > 1f + MaxDelta)
            {
                tmpDelta = 1f + MaxDelta;
            }

            if (Math.Abs(tmpDelta - _lastDelta) > 0.002)
            {
                float newSize;
                
                switch (_snapCamera)
                {
                    case SnapCamera.Bottom:
                        newSize = _defOrthographicSize * tmpDelta;
                        _camera.orthographicSize = newSize;
                        SetBottom(newSize - _defOrthographicSize);     
                        break;
                }     
            }

            _lastDelta = tmpDelta;

            // _tmpDelta = _tmpDelta switch
            // {
            //     < 1f - MaxDelta => 1f - MaxDelta,
            //     > 1f + MaxDelta => 1f + MaxDelta,
            //     _ => defaultDifference / newDifference
            // };

        }
        private void SetBottom(float deltaSize)
        {
            var contentLocalPosition = new Vector3(0,0, _camera.transform.localPosition.z);
            contentLocalPosition.y += deltaSize;
            _camera.transform.localPosition = contentLocalPosition;
        }
        
        private void LeftBottomScale(float deltaSize)
        {
            var contentLocalPosition = new Vector3(0,0, _camera.transform.localPosition.z);
            contentLocalPosition.y += deltaSize;
            contentLocalPosition.x -= deltaSize;
            _camera.transform.localPosition = contentLocalPosition;
        }
    }
    
    public enum SnapCamera
    {
        Bottom,
        Center
    }
}