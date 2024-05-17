using Enums;
using UI.Windows;
using UnityEngine;
using Views.Health;
using Views.Units.Units;

namespace Models.Controllers.Skills
{
    public class Gas: Skill
    {
        protected void Start()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
            _skillCooldownPassed = true;
            UpdateUIBar(1.0f);
        }

        public override void LaunchMissile(Vector3 mousePosition)
        {
            if (_skillCooldownPassed)
            {
                DetectAndDestroyEnemies(mousePosition);
                StartTimer();
            }
           
        }

        protected override void UpdateUIBar(float value)
        {
            _bottomPanelWindow.UpdateGasFill(value);
        }

        public void DetectAndDestroyEnemies(Vector3 mousePosition)
        {
            _explosionRadius = 80f;
            _projectileView.transform.position = mousePosition;
            Collider2D[]
                hitColliders =
                    Physics2D.OverlapCircleAll(_projectileView.transform.position,
                        _explosionRadius,1<<LayerMask.NameToLayer("Enemy")); // Assuming the radius of your aim is 1 unit.
            foreach (var hitCollider in hitColliders)
            {
                hitCollider.gameObject.TryGetComponent(out Damageable damageable);
                PlayParticleOneShot(damageable, (int) _player.Pumping.Skills[SkillTypesEnum.Grenade].Damage);
            }
        }
        public void PlayParticleOneShot(Damageable damageable, int damage)
        {
            _particleSystem.transform.position =new Vector3(_projectileView.position.x, _projectileView.position.y, _particleSystem.transform.position.z);
            _particleSystem.Play();
            if(damageable != null)
                damageable.SetDamage(damage);
        }
    }
}