using System.Collections.Generic;
using UnityEngine;

namespace Views.Move
{
    public class MovementPathGroups : MonoBehaviour
    {
        [SerializeField] private List<MovementPath> _movementPaths;

        public (MovementPath movementPath, int[] pathProperties) GetRandomPath()
        {
            var index = Random.Range(0, _movementPaths.Count);
            return (_movementPaths[index],new int[]{_movementPaths.Count,index});
        }
    }
}