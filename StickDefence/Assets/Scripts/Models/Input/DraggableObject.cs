using Enums;
using Models.Fabrics;
using TMPro;
using TonkoGames.Controllers.Core;
using UnityEngine;
using VContainer;
using Views.Units.Units;

namespace Models.Input
{
    public class DraggableObject : MonoBehaviour
    {
        [Inject] private ConfigManager _configManager;
        [Inject] private PrefabInject _prefabInject;
        private Camera _camera;
        private PlayerView _playerView;
        private Vector2 _mousePosition;
        private float offsetX, offsetY;
        public static bool _mouseButtonReleased;

        private Rigidbody2D rb;
        private Camera mainCamera;
        private bool isDragging = false;
        private Vector3 offset;
        

        void Start()
        {
            // Cache references for performance
            rb = GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;
            _playerView = GetComponent<PlayerView>();
        }

        void Update()
        {
            // Handle mouse input
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                StartDrag();
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }

            // Handle touch input
            if (UnityEngine.Input.touchCount > 0)
            {
                Touch touch = UnityEngine.Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        StartDrag(touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        EndDrag();
                        break;
                }
            }

            if (isDragging)
            {
                Vector3 currentPosition;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
                currentPosition = mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition) + offset;
#else
            currentPosition = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position) + offset;
#endif
                currentPosition.z = 0; // Ensure we're not moving the object in Z-axis
                rb.MovePosition(currentPosition);
            }
        }

        private void StartDrag(Vector3 inputPosition)
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(inputPosition);
            worldPosition.z = 0;
            if (Vector2.Distance(rb.position, worldPosition) < 1f) // Example threshold for starting drag
            {
                isDragging = true;
                offset = transform.position - worldPosition;
            }
        }

        private void StartDrag()
        {
            StartDrag(UnityEngine.Input.mousePosition);
        }

        private void EndDrag()
        {
            isDragging = false;
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerView otherPlayerView))
            {
                if (!isDragging && _playerView.UnitType == otherPlayerView.UnitType &&
                    (int) _playerView.UnitType != 2)
                {
                    GameObject playerUnit = Instantiate(
                        _configManager.PrefabsUnitsSO.PlayerUnitPrefabs[
                            (PlayerUnitTypeEnum) ((int) _playerView.UnitType + 1)],
                        otherPlayerView.transform.position, otherPlayerView.transform.rotation);
                    _prefabInject.InjectGameObject(playerUnit);
                    _mouseButtonReleased = false;
                    Destroy(other.gameObject);
                    Destroy(gameObject);

                }
            }
        }
    }

}