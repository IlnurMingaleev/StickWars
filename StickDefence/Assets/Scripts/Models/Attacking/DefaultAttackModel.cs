using System;
using System.Collections.Generic;
using TonkoGames.Sound;
using Models.Timers;
using UniRx;
using UnityEngine;
using Views.Health;
using Random = UnityEngine.Random;

namespace Models.Attacking
{
    public abstract class DefaultAttackModel
    {
        private const float WaitTickFind = 0.06f;
        
        private float _attackRange;
        private int _cooldownMillisecondsDuration;
        private int _damage;
        private float _criticalChance;
        private float _criticalMultiplier = 1;
        
        protected Transform PosAttack;
        protected Transform PosSpawnProjectile;
        protected IDamageable TargetDamageable;
        
        protected List<IDamageable> SplashDamageables = new();
        protected ITimerService TimerService;
        protected ISoundManager SoundManager;
        
        protected ContactFilter2D ContactFilter;

        protected readonly DefaultAttackingCircle AttackingCircle = new();
        private readonly CompositeDisposable _timerFindDisposable = new();
        private readonly ReloadingTimer _reloadingTimer = new();
        public bool IsEnemyFound  { get; protected set; }

        private bool _isPlay;
        private bool _canFindAttack;
        protected Action _startAttackAnimAction;
        private Action _endCooldownAttackAction;
        
        public int GetDamage()=> _damage;

        public virtual void Init(AttackBlockView attackView, ITimerService timerService, ISoundManager soundManager, Action startAttackAnim, Action endCooldownAttackAction)
        {
            PosAttack = attackView.PosAttack;
            _attackRange = attackView.AttackRange;
            TimerService = timerService;
            ContactFilter = attackView.ContactFilter;
            SoundManager = soundManager;
            AttackingCircle.Init(_attackRange, attackView.ContactFilter, PosAttack);
            _startAttackAnimAction = startAttackAnim;
            _endCooldownAttackAction = endCooldownAttackAction;
            StartLoopUpdate();
        }
        public void ReSetupRangeAttack(float attackRange){
            _attackRange = attackRange;
            AttackingCircle.ReSetupAttackRange(_attackRange);
        }
        public void SetReloading(int reloading) => _cooldownMillisecondsDuration = reloading;
        public void SetDamage(int value) => _damage = value;
        public void SetCriticalChance(float value) => _criticalChance = value;
        public void SetCriticalMultiplier(float value = 1) => _criticalMultiplier = value;
        
        public void StartPlay()
        {
            _isPlay = true;
            if (!_reloadingTimer.IsReloading)
            {
                StartFindAttack();
            }
            else
            {
                _reloadingTimer.StartTick();
            }
        }

        public void StopPlay()
        {
            _isPlay = false;
            StopCanAttacking();
            _reloadingTimer.Clear();
        }
        
        public void SetTargetUnit(IDamageable damageable)
        {
            AttackingCircle.SetTarget(damageable);
        }
        
        public void Dead()
        {
            _canFindAttack = false;
            ClearAllTimers();
        }
        
        public void Resurrect()
        {
            StartLoopUpdate();
        }
        
        private void StartLoopUpdate()
        {
            Observable.Timer(TimeSpan.FromSeconds(WaitTickFind)).Repeat().Subscribe(_ => TickUpdate()).AddTo(_timerFindDisposable);
        }
        
        private void TickUpdate()
        {
            if (_canFindAttack)
            {
                EndFindAttackTick();
            }
        }
        
        protected void StartFindAttack(Action endCooldown = null)
        {
            ClearCooldownTimer();
            
            _canFindAttack = true;
        }
        
        protected virtual void EndFindAttackTick()
        {
        }
        
        public void StartCooldown()
        {
            ClearCooldownTimer();
            
            _reloadingTimer.Init(_cooldownMillisecondsDuration, EndCooldown);

            if (_isPlay)
            {
                _reloadingTimer.StartTick();
            }
        }

        private void EndCooldown()
        {
            _endCooldownAttackAction?.Invoke();
            if (_isPlay)
                StartFindAttack();
        }
  
        protected void StartAttackAnim()
        {
            StopCanAttacking();
            _startAttackAnimAction?.Invoke();
            StartCooldown();
        }
        
        public virtual void Attack()
        {
        }

        private void ClearAllTimers()
        {
            _timerFindDisposable.Clear();
            ClearCooldownTimer();
        }
        
        protected void StopCanAttacking()
        {
            _canFindAttack = false;
        }

        private void ClearCooldownTimer()
        {
            _reloadingTimer.FinishTimer();
        }

        protected bool IsCritical() => Random.Range(0, 100) <= _criticalChance;
        protected int DamageCritical(bool value) => value ? (int)(_damage * _criticalMultiplier) : _damage;
        protected void SetDamage(IDamageable damageable)
        {
            damageable?.SetDamage(DamageCritical(IsCritical()));
        }
        
        ~DefaultAttackModel(){
            ClearAllTimers();
        }
    }
}