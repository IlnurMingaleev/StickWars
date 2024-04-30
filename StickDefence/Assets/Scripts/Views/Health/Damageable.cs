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
        Transform GetTransformCenterPoint();
        Transform GetTransform();
        ReactiveProperty<float> SpeedToCalculatePredict { get; }
    }

    public class Damageable : MonoBehaviour, IDamageable
    {
        [SerializeField] private List<DamageableFlashAnim> _damageableFlash;
        [SerializeField] private Transform _centerPoint;
        private IEnumerator flashingCoroutine;
        private float _armor = 0;
        private bool _isInvulnerability = false;
        
        private ReactiveProperty<int> _healthCurrent = new ReactiveProperty<int>();
        private ReactiveProperty<int> _healthMax = new ReactiveProperty<int>();
        private ReactiveProperty<bool> _isEmptyHealth = new ReactiveProperty<bool>();

        public IReadOnlyReactiveProperty<int> HealthCurrent => _healthCurrent;
        public IReadOnlyReactiveProperty<int> HealthMax => _healthMax;
        public IReadOnlyReactiveProperty<bool> IsEmptyHealth => _isEmptyHealth;

        public Transform GetTransformToCenterPoint() => _centerPoint;
        public Transform GetTransform() => transform;
        public ReactiveProperty<float> _speedToCalculatePredict;
        public ReactiveProperty<float> SpeedToCalculatePredict => _speedToCalculatePredict;

        public void SetSpeedToCalculatePredict(ReactiveProperty<float> speed)
        {
            _speedToCalculatePredict = speed;
        }

        public void Init(int health, float armor, ReactiveProperty<float> speed)
        {
            _healthMax.Value = health;
            _healthCurrent.Value = health;
            _armor = armor;
            _speedToCalculatePredict = speed;
        }

        public void Init(int health, float armor)
        {
            _healthMax.Value = health;
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

               //TODO  damageable flashAnimation
               ///*foreach (var damageableFlashAnim in _damageableFlash)
                    //damageableFlashAnim.Flash();*/
            }
        }

        public void SetInvulnerability(bool value)
        {
            _isInvulnerability = value;
        }

        public void SetMaxHealth(int health)
        {
            if (_healthMax.Value == health)
                return;

            float deltaCurrent = (float)_healthCurrent.Value / (float)_healthMax.Value;
            
            _healthMax.Value = health;

            _healthCurrent.Value = (int) (_healthMax.Value * deltaCurrent);
        }

        public void AddHealth(int health)
        {
            var tmpHealthCurrent = _healthCurrent.Value;
            tmpHealthCurrent += health;

            if (_healthCurrent.Value > _healthMax.Value)
            {
                tmpHealthCurrent = _healthMax.Value;
            }
            
            _healthCurrent.Value = tmpHealthCurrent;
        }

        public void UpdateDefence(float armor)
        {
            _armor = armor;
        }

        public void Resurrect()
        {
            _healthCurrent.Value = _healthMax.Value;
            _isEmptyHealth.Value = false;
        }
    }
}