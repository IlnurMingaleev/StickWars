﻿using System;
using Anim.Battle.Fortress;
using Models.Attacking;
using Models.Battle;
using Models.Fortress;
using UniRx;
using UnityEngine;
using Views.Health;

namespace Views.Units.Fortress
{
    public class PlayerUnitView : MonoBehaviour
    {
        [SerializeField] private BattleFortressLaunch _battleFortressLaunch;
        [SerializeField] private Damageable _damageable;
        [SerializeField] private AttackBlockView _attackBlockView;
        [SerializeField] private MeshRenderer _meshRenderer;

        private ReactiveProperty<bool> _isActive = new ReactiveProperty<bool>(false);
        public Damageable Damageable => _damageable;
        public AttackBlockView AttackBlockView => _attackBlockView;

        public IReadOnlyReactiveProperty<int> HealthCurrent => _damageable.HealthCurrent;
        public IReadOnlyReactiveProperty<int> HealthMax => _damageable.HealthMax;
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;
          
        public bool IsLaunchIsProgress => _battleFortressLaunch.IsLaunchIsProgress;

        private PlayerUnitsBuilder _playerUnitsBuilder;
        private PlayerUnitModel _playerUnitModel;

        public MeshRenderer MeshRenderer => _meshRenderer;
        private void OnEnable()
        {
            _isActive.Value = true;
        }

        private void OnDisable()
        {
            _isActive.Value = false;
        }
        
        public void StartPrepare()
        {
            _battleFortressLaunch.StartPrepare();
        }
        
        public void StartAttackAnim()
        {
            _battleFortressLaunch.StartLaunchAnim();
        }

        public void OnPause()
        {
            _battleFortressLaunch.Animator.enabled = false;
        }

        public void OnPlay()
        {
            _battleFortressLaunch.Animator.enabled = true;
        }

        public void SetStickmanMaterial(Material material)
        {
            if (_meshRenderer != null) _meshRenderer.material = material;
        }


        public void SetSortingOrder(int sortOrder)
        {
            _meshRenderer.sortingOrder = sortOrder;
        }
    }
}