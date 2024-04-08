using System;
using System.Collections.Generic;
using UI.UIManager;
using UI.Windows;
using UnityEngine;
using VContainer;

namespace Models.Battle.Boosters
{
    public class BoosterDrone: MonoBehaviour
    { 
        [SerializeField]private Transform[] waypoints;
        [SerializeField] private float _speed;
        private const float _epsilon = 0.001f;
        private int waypointIndex = 0;
        private bool _isMoving = false;
        private Camera _mainCamera;
        public Action AdDroneMoveEnd;
        private bool _boosterWindowShown = false;
        [Inject] private IWindowManager _windowManager;
        
        
        public void Start()
        {
            _isMoving = true;
            _mainCamera = Camera.main;
        }

        public void Update()
        {
            if(_isMoving)
                MoveToWaypoint();
            if (UnityEngine.Input.GetMouseButtonDown(0))
                SendRaycast();
        }
        private void MoveToWaypoint()
        {
            float step = _speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].position, step);

            if (Vector3.Distance(transform.position, waypoints[waypointIndex].position) < _epsilon)
            {
                waypointIndex++;
                if (waypointIndex == waypoints.Length)
                {
                    _isMoving = false;
                    waypointIndex = 0;
                    _boosterWindowShown = false;
                    AdDroneMoveEnd?.Invoke();
                }
            }
        }

        public void SendRaycast()
        {
            RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.TryGetComponent(out BoosterDrone boosterDrone))
                {
                    boosterDrone.ShowBoosterWindow();
                }
            }
        }

        private void ShowBoosterWindow()
        {
            _windowManager.GetWindow<BottomPanelWindow>();
        }
    }
}