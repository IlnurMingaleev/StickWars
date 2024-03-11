using UnityEngine;

namespace Views.Cameras
{
    public class CameraSetBasicSize : MonoBehaviour
    {
        [SerializeField] private float _sizeCamera;
        
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            if (_camera != null) _camera.orthographicSize = _sizeCamera;
        }
    }
}