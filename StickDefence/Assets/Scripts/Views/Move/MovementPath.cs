using Models.Move;
using UnityEngine;

namespace Views.Move
{
    public class MovementPath : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private PathTypesEnum _pathTypes;
        [SerializeField] private Transform[] _pathElements;

        public PathTypesEnum PathTypes => _pathTypes;
        public Transform[] PathElements => _pathElements;
        public Transform SpawnPoint => _spawnPoint;
        
#if UNITY_EDITOR
        [Header("Editor")]
        [SerializeField] private Color PathColor;

        private void OnDrawGizmos()
        {
            if (_pathElements == null || _pathElements.Length < 2)
            {
                return;
            }
            
            Gizmos.color = PathColor;
            
            for (int i = 1; i < _pathElements.Length; i++)
            {
                Gizmos.DrawLine(_pathElements[i - 1].position, _pathElements[i].position);
            }

            if (_pathTypes == PathTypesEnum.Loop)
            {
                Gizmos.DrawLine(_pathElements[0].position, _pathElements[_pathElements.Length - 1].position);
            }
        }
#endif
    }
}