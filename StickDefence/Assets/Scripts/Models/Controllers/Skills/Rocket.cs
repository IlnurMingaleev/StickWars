using System.Collections;
using Enums;
using UI.Windows;
using UnityEngine;
using Views.Health;

namespace Models.Controllers.Skills
{
    public class Rocket: Skill
    {
        [SerializeField] private float duration = 4.0f;
        [SerializeField] private Transform _start;

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
                _skillCooldownPassed = false;
                StartCoroutine(Curve(mousePosition));
                StartTimer();
            }
           
        }

        protected override void UpdateUIBar(float value)
        {
            _bottomPanelWindow.UpdateRocketFill(value);
        }

        public void DetectAndDestroyEnemies()
        {
            _explosionRadius = 4f;
            Collider2D[]
                hitColliders =
                    Physics2D.OverlapCircleAll(_projectileView.transform.position,
                        _explosionRadius,1<<LayerMask.NameToLayer("Enemy")); // Assuming the radius of your aim is 1 unit.
            foreach (var hitCollider in hitColliders)
            {
                hitCollider.gameObject.TryGetComponent(out Damageable damageable);
                PlayParticleOneShot(damageable, (int) _player.Pumping.Skills[SkillTypesEnum.Grenade].Damage);
                _soundManager.PlayExplosionOneShot();
            }

        }
        public IEnumerator Curve(Vector3 finish)
        {
            _projectileView.gameObject.SetActive(true);
            var timePast = 0f;
            //temp vars
            while (timePast < duration)
            {
                timePast += Time.deltaTime;

                var linearTime =  timePast / duration;
                _projectileView.position =
                    Vector3.Lerp(_start.position, finish, linearTime);

                yield return null;
            }

            _projectileView.gameObject.SetActive(false);
            DetectAndDestroyEnemies();
        }
        public void PlayParticleOneShot(Damageable damageable, int damage)
        {
            _particleSystem.transform.position =new Vector3(_projectileView.position.x, _projectileView.position.y, _particleSystem.transform.position.z);
            _particleSystem.Play();
            if(damageable != null)
                damageable.SetDamage(damage);
        }
        
        protected override void ActivateSkillButton()
        {
            _bottomPanelWindow.SetRocketBtnInteractability(true);
        }

        protected override void DeacivateSkillButton()
        {
            _bottomPanelWindow.SetRocketBtnInteractability(false);
        }

    }
}