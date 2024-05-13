using UnityEngine;

namespace Tools.GameTools
{
    public class MatchWidth : MonoBehaviour
    {
        public float sceneWidth = 165;

        Camera _camera;
        void Start() {
            _camera = GetComponent<Camera>();
        }

        // Adjust the camera's height so the desired scene width fits in view
        // even if the screen/window size changes dynamically.
        void FixedUpdate() {
            float unitsPerPixel = sceneWidth / Screen.width;

            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            _camera.orthographicSize = desiredHalfHeight;
        }
    }
}