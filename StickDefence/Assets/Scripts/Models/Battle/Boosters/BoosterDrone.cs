using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Battle.Boosters
{
    public class BoosterDrone: MonoBehaviour
    { 
        [SerializeField]private Transform[] waypoints;
        [SerializeField] private float _speed;
        private const float _epsilon = 0.001f;
        private int waypointIndex = 0;
        private bool _isMoving = false;
        public Action AdDroneMoveEnd;
        
        public void Start()
        {
            _isMoving = true;
        }

        public void Update()
        {
            if(_isMoving)
                MoveToWaypoint();
        }
        void MoveToWaypoint()
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
                    AdDroneMoveEnd?.Invoke();
                }
            }
        }
    
    }
}