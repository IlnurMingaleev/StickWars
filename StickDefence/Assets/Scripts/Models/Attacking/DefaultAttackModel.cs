using System;
using TonkoGames.Sound;
using Models.Timers;
using UniRx;
using UnityEngine;
using Views.Health;

namespace Models.Attacking
{
    public abstract class DefaultAttackModel
    {
          private const float WaitTickFind = 0.02f;
        
        private float _attackRange;
        private float _cooldownDuration;
        protected int Damage;
        protected Transform PosAttack;
        protected IDamageable TargetDamageable;
        protected ITimerService TimerService;
        protected ISoundManager SoundManager;
        
        protected ContactFilter2D ContactFilter;

        protected readonly DefaultAttackingCircle AttackingCircle = new();
        protected ITimerModel TimerModelCooldown;
        protected Action StartAttackAnimAction;
        protected CompositeDisposable _timerFindDisposable = new CompositeDisposable();
        protected CompositeDisposable _timerAttackDisposable = new CompositeDisposable();
        private bool _isFind = false;
        protected bool IsEnemyFinded = false;

        private bool _isPlay;
        
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
        }

        public void ReSetupRangeAttack(float attackRange)
        {
            _attackRange = attackRange;
        }

        public void SetReloading(float reloading)
        {
            _cooldownDuration = reloading;
        }
        
        public void SetDamage(int value)
        {
            Damage = value;
        }

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
            ClearAllTimers();
        }

        protected void StartFindAttack()
        {
            ClearAllTimers();
            Observable.Timer(TimeSpan.FromSeconds(WaitTickFind)).Repeat().Subscribe(_ => EndFindAttackTick()).AddTo(_timerFindDisposable);
        }

        protected virtual void EndFindAttackTick()
        {
        }

        public void StartCooldown(Action endCooldown)
        {
            ClearAllTimers();
            IsEnemyFinded = false;
            Observable.Timer(TimeSpan.FromSeconds(_cooldownDuration)).Repeat().Subscribe(_ =>EndCooldown() ).AddTo(_timerAttackDisposable);
            /*TimerModelCooldown = TimerService.AddGameTimer(_cooldownDuration, null, () =>
            {
                endCooldown?.Invoke();
                EndCooldown();
            }, false)*/;
        }

        private void EndCooldown()
        {
            if (_isPlay)
            {
                StartFindAttack();
            }
        }
  
        public virtual void Attack()
        {
        }

        private void ClearAllTimers()
        {
            ClearFindAttackTimer();
            ClearCooldownTimer();
        }
        
        protected void ClearFindAttackTimer()
        {
            _timerFindDisposable.Clear();
            _isFind = false;
        }

        private void ClearCooldownTimer()
        {
            /*if (TimerModelCooldown != null)
            {
               // TimerModelCooldown.StopTick();
                TimerModelCooldown = null;
            }*/
            _timerAttackDisposable.Clear();
        }
    }
}