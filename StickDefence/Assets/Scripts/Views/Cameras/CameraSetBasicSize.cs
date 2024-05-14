using UnityEngine;

namespace Views.Cameras
{
    public class CameraSetBasicSize : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField] private float _sizeCamera;

            private void Start()
        {
            _camera = Camera.main;

            if (_camera != null) _camera.orthographicSize = _sizeCamera;
        }
    }
}