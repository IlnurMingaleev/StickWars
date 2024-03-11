using Tools.Extensions;
using UnityEngine;

namespace Models.Move
{
    public class TopDownMove
    {
        private readonly Transform _selfTargetBody;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly float _speed;
        private readonly Transform _bodyScale = null;
        private Vector2 _cashVelocity;
        
        public TopDownMove(Transform targetBody, Rigidbody2D rigidbody2D, float speed, Transform bodyScale = null)
        {
            _speed = speed;
            _rigidbody2D = rigidbody2D;
            _selfTargetBody = targetBody;
            _bodyScale = bodyScale;
        }

        public void StopMove()
        {
            _rigidbody2D.velocity = Vector2.zero;
        }

        public void ContinueMove()
        {
            _rigidbody2D.velocity = _cashVelocity;
        }
        
        public void MoveXForSpeed()
        {
            _rigidbody2D.velocity = new Vector2( _speed, 0);
        }
        
        public void CalculateMove(Vector3 target)
        {
            _selfTargetBody.transform.LookAt2D(target);

            var direction = target - _selfTargetBody.position;
            
            direction = direction.normalized;
            
            if (_bodyScale != null)
            {
                if (direction.x >= 0)
                {
                    var transformLocalScale = _bodyScale.transform.localScale;
                    transformLocalScale.x = 1;
                    _bodyScale.localScale = transformLocalScale;
                }
                else
                {
                    var transformLocalScale = _bodyScale.transform.localScale;
                    transformLocalScale.x = -1;
                    _bodyScale.localScale = transformLocalScale;
                }
            }

            _cashVelocity = new Vector2(direction.x * _speed, direction.y * _speed);
        }
    }
}