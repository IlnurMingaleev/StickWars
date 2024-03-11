using System;
using Models.SO.Core;
using UniRx;
using Views.Units.Units;

namespace Models.Units
{
    public class BaseUnit
    {
        public readonly UnitView View;

        private Action<BaseUnit> UnitKilledAction;

        private bool _isPlayable = false;
        private CompositeDisposable _disposable = new CompositeDisposable();
        protected UnitStatsConfig UnitStatsConfig;
        
        public BaseUnit(UnitView unitView) 
        {
            View = unitView;
            unitView.InitUnityActions(OnEnable, OnDisable);
        }

        public void InitActions(Action<BaseUnit> unitKilled)
        {
            UnitKilledAction = unitKilled;
        }

        public void InitUnitConfigStats(UnitStatsConfig unitStatsConfig)
        {
            UnitStatsConfig = unitStatsConfig;
            View.Damageable.Init(unitStatsConfig.Health, unitStatsConfig.Armor);
        }

        protected virtual void OnEnable()
        {
            View.Damageable.IsEmptyHealth.Subscribe(OnDead).AddTo(_disposable);
        }

        protected virtual void OnDisable()
        {
            _disposable.Clear();
        }

        public void OnPlay()
        {
            _isPlayable = true;
        }

        public void OnPause()
        {
            _isPlayable = false;
        }

        public void Update()
        {
            if (!_isPlayable)
                return;
            
            View.UnitFollowPath.Move();
        }

        protected virtual void OnDead(bool value)
        {
            
        }

        protected virtual void OnWalk(bool value)
        {
            
        }
    }
}