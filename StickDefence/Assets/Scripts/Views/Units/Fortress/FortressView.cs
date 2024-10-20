﻿using System;
using Anim.Battle.Fortress;
using Models.Attacking;
using TMPro;
using UniRx;
using UnityEngine;
using Views.Health;

namespace Views.Units.Fortress
{
    public class FortressView : MonoBehaviour
    {
        [SerializeField] private Damageable _damageable;
        [SerializeField] private AttackBlockView _attackBlockView;
        [SerializeField] private TMP_Text _levelLabel;

        private ReactiveProperty<bool> _isActive = new ReactiveProperty<bool>(false);
        
        public Damageable Damageable => _damageable;
        public ReactiveProperty<float> Speed  = new ReactiveProperty<float>(0);

        public IReadOnlyReactiveProperty<int> HealthCurrent => _damageable.HealthCurrent;
        public IReadOnlyReactiveProperty<int> HealthMax => _damageable.HealthMax;
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;
          
        public void SetLevelLabel(int level)
        {
            _levelLabel.text = $"{level + 1}";
        }

        private void OnEnable()
        {
            _isActive.Value = true;
            _damageable.SetSpeedToCalculatePredict(Speed);
        }

        private void OnDisable()
        {
            _isActive.Value = false;
        }
        
        public void RessurectHealth()
        {
            
        }
    }
}