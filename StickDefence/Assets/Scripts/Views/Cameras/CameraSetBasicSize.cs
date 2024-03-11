using UnityEngine;

namespace Views.Cameras
{
    public class CameraSetBasicSize : MonoBehaviour
    {
        [SerializeField] private float _sizeCamera;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            
        }
    }
}