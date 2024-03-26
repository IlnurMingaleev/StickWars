using System;
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

        private Rigidbody2D rb;
        private Camera mainCamera;
        private bool _mouseButtonReleased = false;
        private Vector3 offset;
        [SerializeField] private LayerMask _layer;
        private bool isDragging;


        void Start()
        {
            // Cache references for performance
            rb = GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;
            _playerView = GetComponent<PlayerView>();
        }
        

        private void OnMouseDown()
        {
            StartDrag();
        }

        private void OnMouseUp()
        {
            EndDrag();
        }

        private void OnMouseDrag()
        {
            if (isDragging)
            {
                transform.position = new Vector3(mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition).x,
                    mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition).y, transform.position.z);
            }
        }

        void UpdateOld()
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
            isDragging =false;
            var hit2d = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition),
                mainCamera.transform.forward, 10f, _layer);

            if (hit2d)
            {
                if(hit2d.collider.TryGetComponent(out PlayerView playerView) 
                   && _playerView.UnitType == playerView.UnitType 
                   && _playerView.UnitType != PlayerUnitTypeEnum.PLayerThree
                   && hit2d.collider.gameObject != gameObject) 
                {
                    Debug.Log("This is what we hit" + hit2d.collider);
                    var transform1 = hit2d.collider.transform;
                    GameObject go =
                        Instantiate(
                            _configManager.PrefabsUnitsSO.PlayerUnitPrefabs[
                               (PlayerUnitTypeEnum)((int)_playerView.UnitType+1)].GO,
                            transform1.position,
                            transform1.rotation); 
                    _prefabInject.InjectGameObject(go);
                    Destroy(hit2d.collider.gameObject);
                    Destroy(gameObject);
                }
            }
        }
        


        /*private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerView otherPlayerView))
            {
                if (_mouseButtonReleased && _playerView.UnitType == otherPlayerView.UnitType &&
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
        }*/
    }

}