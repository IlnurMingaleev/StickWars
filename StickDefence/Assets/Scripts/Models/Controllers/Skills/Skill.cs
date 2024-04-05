using Models.Player;
using UnityEngine;
using UnityEngine.WSA;
using VContainer;
using Views.Health;
using Views.Units.Units;

namespace Models.Controllers.Skills
{
    public abstract class Skill : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem _particleSystem;
        [SerializeField] protected Transform _projectileView;
        [SerializeField] protected Transform _aimView;
        [Inject] protected IPlayer _player;
        protected float _explosionRadius;

        public Transform AimView
        {
            get { return _aimView; }
            set { _aimView = value; }
        }


        public abstract void LaunchMissile(Vector3 mousePosition);

        public abstract void DetectAndDestroyEnemies();
    }
}