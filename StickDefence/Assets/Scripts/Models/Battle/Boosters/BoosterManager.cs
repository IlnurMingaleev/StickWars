using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Models.Merge;
using Models.Timers;
using TonkoGames.StateMachine.Enums;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using VContainer;

namespace Models.Battle.Boosters
{
    public class BoosterManager: MonoBehaviour, IDisposable
    {
        [SerializeField] private MergeController _mergeController;
        [FormerlySerializedAs("_playerUnitsBuilderTwo")] [SerializeField] private PlayerUnitsBuilder playerUnitsBuilder;
        [SerializeField] private MapUnitsBuilder _mapUnitsBuilder;
        [SerializeField] private BoosterDrone _boosterDrone;
        [SerializeField] private CoroutineTimer[] _boosterTimers;
        [Inject] private IWindowManager _windowManager;
        [Inject] private ITimerService _timerService;
        private const float _rewardedCooldown = 2f;
        private CompositeDisposable _disposable = new CompositeDisposable();


        private void Start()
        {
            StartDrone();
            _boosterDrone.IsMoving.Subscribe(_ => RestartDroneMovement(_)).AddTo(_disposable);
        }

        private void StartDrone()
        {
            _boosterDrone.StartDrone();
        }

        private void RestartDroneMovement(bool value)
        {
            if (!value) _timerService.AddDefaultTimer(_rewardedCooldown, f => { },
                () => { StartDrone();});
        }

        private Dictionary<BoosterTypeEnum, Booster> _activeBoosters = new Dictionary<BoosterTypeEnum,Booster>();

        public void ApplyBooster(BoosterTypeEnum boosterTypeEnum, Action OnBoosterTimerEnd = null)
        {
            
            if (_activeBoosters.ContainsKey(boosterTypeEnum))
            {
               _activeBoosters[boosterTypeEnum].UpdateExistingTimer();
            }
            else
            {
                _activeBoosters.Add(boosterTypeEnum, CreateNewBooster(boosterTypeEnum));
                _activeBoosters[boosterTypeEnum].CreateNewTimerModel(()=>
                {
                    _activeBoosters[boosterTypeEnum].Dispose();
                    _activeBoosters.Remove(boosterTypeEnum);
                    OnBoosterTimerEnd?.Invoke();
                }, boosterTypeEnum);
            }

        }

        public void OnRuntimeStateSwitch(RunTimeStateEnum runTimeState)
        {
            switch (runTimeState)
            {
                case  RunTimeStateEnum.Pause:
                    break;
                case RunTimeStateEnum.Play:
                    break;
            }
        }

        public Booster CreateNewBooster(BoosterTypeEnum boosterTypeEnum)
        {
            Booster result = null;
            switch (boosterTypeEnum)
            {
                case BoosterTypeEnum.AttackSpeed:
                    result= new AttackSpeed(this, _boosterTimers[0], _windowManager, playerUnitsBuilder);
                    break;
                case BoosterTypeEnum.AutoMerge:
                    result = new AutoMerge(this,  _boosterTimers[1], _windowManager,_mergeController);
                    break;
                case BoosterTypeEnum.GainCoins:
                    result =  new GainMoney(this, _boosterTimers[2], _windowManager,_mapUnitsBuilder);
                    break;
                
            }

            return result;
        }

        public void OnPlay()
        {
            foreach (var booster in _activeBoosters.Values)
            {
                booster.OnPlay();
            }
        }
        public void OnPause()
        {
            foreach (var booster in _activeBoosters.Values)
            {
                booster.OnPause();
            }
        }

        public void CoroutineStart(IEnumerator enumerator)
        {
            StartCoroutine(enumerator);
        }

        public void CoroutineStop(IEnumerator enumerator)
        {
            StopCoroutine(enumerator);
        }

        public void Dispose()
        {
            _disposable.Clear();
        }
    }
}