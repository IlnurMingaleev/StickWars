using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Views.Health
{
    public interface IDamageable
    {
        void SetDamage(int damage);
        Transform GetTransform();
    }

    public class Damageable : MonoBehaviour, IDamageable
    {
        [SerializeField] private List<DamageableFlashAnim> _damageableFlash;
        
        private IEnumerator flashingCoroutine;
        private int _healthBase = 0;
        private float _armor = 0;
        private bool _isInvulnerability = false;
        
        private ReactiveProperty<int> _healthCurrent = new ReactiveProperty<int>();
        private ReactiveProperty<bool> _isEmptyHealth = new ReactiveProperty<bool>();

        public IReadOnlyReactiveProperty<int> HealthCurrent => _healthCurrent;
        public IReadOnlyReactiveProperty<bool> IsEmptyHealth => _isEmptyHealth;
        
        public void Init(int health, float armor)
        {
            _healthBase = health;
            _healthCurrent.Value = health;
            _armor = armor;
        }
        
        public void SetDamage(int value)
        {
            if (_isInvulnerability)
                return;
            
            value -= (int)(_armor * value);
            
            if (_healthCurrent.Value - value <= 0)
            {
                _healthCurrent.Value = 0;
                _isEmptyHealth.Value = true;
                
                foreach (var damageableFlashAnim in _damageableFlash)
                    damageableFlashAnim.StopFlash();
            }
            else
            {
                _healthCurrent.Value -= value;

                foreach (var damageableFlashAnim in _damageableFlash)
                    damageableFlashAnim.Flash();
            }
        }

        public void SetInvulnerability(bool value)
        {
            _isInvulnerability = value;
        }
        
        public void Resurrect()
        {
            _healthCurrent.Value = _healthBase;
            _isEmptyHealth.Value = false;
        }

        public Transform GetTransform() => transform;
    }
}