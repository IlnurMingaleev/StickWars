using System;
using System.Collections.Generic;
using UI.UIManager;
using Ui.Windows;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;

namespace Models.Battle.Boosters
{
    public class BoosterDrone : MonoBehaviour
    {
        [SerializeField] private BoosterManager _boosterManager;
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float _speed;
        private const float _epsilon = 0.001f;
        private int waypointIndex = 0;
        private ReactiveProperty<bool> _isMoving = new ReactiveProperty<bool>(false);
        private Camera _mainCamera;
        public Action AdDroneMoveEnd;
        private bool _boosterWindowShown = false;
        [Inject] private IWindowManager _windowManager;
        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;

        public void Start()
        {
            _mainCamera = Camera.main;
        }

        public void StartDrone()
        {
            _isMoving.Value = true;
            
        }

        public void Update()
        {
            if(_isMoving.Value)
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
                    _isMoving.Value = false;
                    waypointIndex = 0;
                    transform.position = waypoints[0].position;
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
                    transform.position = waypoints[0].position;
                    _isMoving.Value = false;
                    waypointIndex = 0;
                    boosterDrone.ShowBoosterWindow();
                }
            }
        }

        private void ShowBoosterWindow()
        {
            BoosterWindow boosterWindow = _windowManager.GetWindow<BoosterWindow>();
            boosterWindow.Init(_boosterManager);
            _windowManager.Show(boosterWindow);
        }
    }
}