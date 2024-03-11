using System.Collections.Generic;
using UnityEngine;

namespace Views.Move
{
    public class MovementPathGroups : MonoBehaviour
    {
        [SerializeField] private List<MovementPath> _movementPaths;

        public MovementPath GetRandomPath()
        {
            return _movementPaths[Random.Range(0, _movementPaths.Count)];
        }
    }
}