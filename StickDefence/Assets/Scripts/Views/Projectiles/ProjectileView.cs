using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using TonkoGames.Sound;
using Models.Move;
using Models.Timers;
using UnityEditor;
using UnityEngine;
using Views.Health;
using Object = UnityEngine.Object;

namespace Views.Projectiles
{
    public class ProjectileView : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private float _speed;
        [field: SerializeField] public int DeathTime { get; private set; } = 3;  
        
        [Header("Visual")]
        [SerializeField] protected ParticleSystem _particlePrefabTrail;
        [SerializeField] protected ParticleSystem _particlePrefabHit;
        [SerializeField] protected ParticleSystem _particlePrefabDead;
        
        [Header("Audio")]
        [SerializeField] private AudioClip _hitTargetClip;

        [Header("Bullet Dispose Time")] [SerializeField]
        private float _disposeTime = 2.0f;
        private Action<ProjectileView> _bulletDestroyedAction;
        
        protected int Damage = 0;
        private TopDownMove _topDownMove;
        private ISoundManager _soundManager;

        private LayerMask _layerMask;
        private ITimerModel _timerModel;
        private bool _isCritical;
        public float Speed => _speed;
        private bool _isHit;

        private void Awake()
        {
            if (_particlePrefabTrail != null)
            {
                Instantiate(_particlePrefabTrail, transform);
            }
        }

        public void Init(int damage, Vector3 damageablePosition, bool isCritical,
            LayerMask attackMask, ISoundManager soundManager, Action<ProjectileView> bulletDestroyed,
            ITimerService timerService)
        {
            Damage = damage;
            _isCritical = isCritical;
            
            _topDownMove = new TopDownMove(transform, _rigidbody2D, _speed);
            _topDownMove.CalculateMove(damageablePosition);
            _layerMask = attackMask;
            _soundManager = soundManager;
            _bulletDestroyedAction = bulletDestroyed;
            
            _timerModel = timerService.AddGameTimer(DeathTime, null, () =>
            {
                _bulletDestroyedAction?.Invoke(this);
                if(this != null)
                    Destroy(gameObject);
            });
        }

        public void StartMove()
        {
            _topDownMove.ContinueMove();
        }

        public void StopMove()
        {
            _topDownMove.StopMove();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (_isHit)
                return;
            
            if ((_layerMask.value & (1 << other.gameObject.layer)) > 0)
            {
                _isHit = true;
                OnHit(other);

                _timerModel?.StopTick();
                _bulletDestroyedAction?.Invoke(this);
                _topDownMove.Dispose();
                Destroy(gameObject);
            }
        }

        protected void InstantiateParticle(ParticleSystem particle)
        {
            if (particle != null)
            {
                Instantiate(particle, transform.position, Quaternion.identity);
            }
        }

        protected void PlayHitTarget()
        {
            if (_hitTargetClip != null)
            {
                _soundManager.PlayExplosionSourceOneShot(_hitTargetClip);
            }
        }

        protected virtual void OnHit(Collider2D other)
        {
        }

        private void OnDisable()
        {
            if(_topDownMove!= null) _topDownMove.Dispose();
        }
        
        public void DisposeTopDownMove()
        {
            if(_topDownMove!= null)_topDownMove.Dispose();
        }
        
    }
}