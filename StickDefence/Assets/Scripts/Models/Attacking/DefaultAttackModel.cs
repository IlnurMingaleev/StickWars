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
        private float _cooldownDuration;
        protected int Damage;
        protected float CriticalChance;
        protected float CriticalMultiplier = 1;
        
        protected Transform PosAttack;
        protected Transform PosSpawnProjectile;
        protected IDamageable TargetDamageable;
        
        protected List<IDamageable> SplashDamageables = new();
        protected ITimerService TimerService;
        protected ISoundManager SoundManager;
        
        protected ContactFilter2D ContactFilter;

        protected readonly DefaultAttackingCircle AttackingCircle = new();
        protected ITimerModel TimerModelCooldown;
        protected Action StartAttackAnimAction;
        protected CompositeDisposable _timerFindDisposable = new CompositeDisposable();
        protected bool IsEnemyFinded = false;

        private bool _isPlay;
        private bool _canFindAttack;
        
        public int GetDamage()=> Damage;

        public virtual void Init(AttackBlockView attackView, ITimerService timerService, ISoundManager soundManager, Action startAttackAnim)
        {
            PosAttack = attackView.PosAttack;
            _attackRange = attackView.AttackRange;
            TimerService = timerService;
            ContactFilter = attackView.ContactFilter;
            SoundManager = soundManager;
            AttackingCircle.Init(_attackRange, attackView.ContactFilter, PosAttack);
            StartAttackAnimAction = startAttackAnim;
            StartLoopUpdate();
        }
        public void ReSetupRangeAttack(float attackRange) => _attackRange = attackRange;
        public void SetReloading(float reloading) => _cooldownDuration = reloading;
        public void SetDamage(int value) => Damage = value;
        public void SetCriticalChance(float value) => CriticalChance = value;
        public void SetCriticalMultiplier(float value = 1) => CriticalMultiplier = value;
        
        public void StartPlay()
        {
            _isPlay = true;
            if (!IsEnemyFinded && TimerModelCooldown == null)
            {
                StartFindAttack();
            }
        }

        public void StopPlay()
        {
            _isPlay = false;
            StopCanAttacking();
        }

        public void Dead()
        {
            StopPlay();
            ClearAllTimers();
        }
        
        protected void StartLoopUpdate()
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
            var result = EndFindAttackTick();
            
            if (!result)
            {
                endCooldown?.Invoke();
                _canFindAttack = true;
            }
        }
        
        protected virtual bool EndFindAttackTick()
        {
            return false;
        }
        
        public void StartCooldown(Action endCooldown)
        {
            ClearCooldownTimer();
            IsEnemyFinded = false;
            TimerModelCooldown = TimerService.AddGameTimer(_cooldownDuration, null, () =>
            {
                EndCooldown(endCooldown);
            }, false);
            
        }

        private void EndCooldown(Action endCooldown)
        {
            TimerModelCooldown = null;
            
            if (_isPlay)
            {
                StartFindAttack(endCooldown);
            }
            else
            {
                endCooldown?.Invoke();
            }
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
            if (TimerModelCooldown != null)
            {
                TimerModelCooldown.StopTick();
                TimerModelCooldown = null;
            }
        }

        protected bool IsCritical() => Random.Range(0, 100) <= CriticalChance;
        protected int DamageCritical(bool value) => value ? (int)(Damage * CriticalMultiplier) : Damage;
        protected void SetDamage(IDamageable damageable)
        {
            damageable?.SetDamage(DamageCritical(IsCritical()));
        }
        
        ~DefaultAttackModel(){
            ClearAllTimers();
        }
    }
}