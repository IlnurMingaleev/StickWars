using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Views.Health;

namespace Models.Attacking
{
    public class DefaultAttackingCircle
    {
        private float _attackRange;
        private ContactFilter2D _contactFilter2D;
        private Transform _posAttack;
        private readonly List<Collider2D> _results = new List<Collider2D>(new Collider2D[2]);
        private IDamageable _targetDamageable;

        public void Init(float attackRange, ContactFilter2D ContactFilter2D, Transform posAttack)
        {
            _attackRange = attackRange;
            _contactFilter2D = ContactFilter2D;
            _posAttack = posAttack;
        }

        public IReadOnlyList<Collider2D> GetAllEntity()
        {
            Find();

            return _results;
        }
        public void ReSetupAttackRange(float attackRange)
        {
            _attackRange = attackRange;
        }
        public IDamageable GetFirstEntity()
        {
            Find();

            if (_results.First() == null) return null;
            return _results.First().TryGetComponent(out IDamageable iDamageable) ? iDamageable : null;
        }

        public void SetTarget(IDamageable damageable)
        {
            _targetDamageable = damageable;
        }
        
        public IDamageable GetNearestEntity(Vector2 position)
        {
            if (IsTargetClosest(position))
            {
                return _targetDamageable;
            }
            
            Find();
            
            var closestCollider = FindClosestEntity(position); 
                
            if (closestCollider != null && closestCollider.TryGetComponent<IDamageable>(out IDamageable iDamageable))
            {
                return iDamageable;
            }
            
            return null;
        }

        private Collider2D FindClosestEntity(Vector2 position) {
            var distance = Mathf.Infinity;
            Collider2D closest = null;
            
            foreach (var entity in _results)
            {
                if (entity == null)
                {
                    continue;
                }
                
                Vector2 diff =  (Vector2) entity.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                
                if(curDistance < distance) {
                    closest = entity;
                    distance = curDistance;
                }
            }
            
            return closest;
        }
        private bool IsTargetClosest(Vector2 position)
        {
            if (_targetDamageable == null)
            {
                return false;
            }
            
            var diff =  (Vector2) _targetDamageable.GetTransformCenterPoint().position - position;
            return diff.magnitude < _attackRange;
        }
        
        
        private void Find() => Physics2D.OverlapCircle(_posAttack.position, _attackRange, _contactFilter2D, _results);
    }
}