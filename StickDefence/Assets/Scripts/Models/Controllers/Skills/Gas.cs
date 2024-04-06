using Enums;
using UnityEngine;
using Views.Health;
using Views.Units.Units;

namespace Models.Controllers.Skills
{
    public class Gas: Skill
    {
        public override void LaunchMissile(Vector3 mousePosition)
        {
            if (_skillCooldownPassed)
            {
                _skillCooldownPassed = false;
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
            _projectileView.transform.position = mousePosition;
            _explosionRadius = 3f;
            Collider2D[]
                hitColliders =
                    Physics2D.OverlapCircleAll(_projectileView.transform.position,
                        _explosionRadius); // Assuming the radius of your aim is 1 unit.
            foreach (var hitCollider in hitColliders)
            {
                hitCollider.gameObject.TryGetComponent(out IDamageable damageable);
                PlayParticleOneShot(damageable, (int) _player.Pumping.Skills[SkillTypesEnum.Grenade].Damage);
            }
        }
        public void PlayParticleOneShot(IDamageable damageable, int damage)
        {
            _particleSystem.transform.position =new Vector3(_projectileView.position.x, _projectileView.position.y, _particleSystem.transform.position.z);
            _particleSystem.Play();
            if(damageable != null)
                damageable.SetDamage(damage);
        }
    }
}