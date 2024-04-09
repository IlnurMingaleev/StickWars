using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Models.Merge;
using Models.Timers;
using UI.UIManager;
using UI.Windows;
using UnityEngine;
using VContainer;

namespace Models.Battle.Boosters
{
    public class BoosterManager: MonoBehaviour
    {
        [SerializeField] private PlayerUnitsBuilder _unitsBuilder;
        [SerializeField] private MergeController _mergeController;
        [SerializeField] private PlayerUnitsBuilderTwo _playerUnitsBuilderTwo;
        [Inject] private IWindowManager _windowManager;
        [Inject] private ITimerService _timerService;
        
        
       
        private Dictionary<BoosterTypeEnum, Booster> _activeBoosters = new Dictionary<BoosterTypeEnum,Booster>();

        public void ApplyBooster(BoosterTypeEnum boosterTypeEnum)
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
                });
            }
            if(_activeBoosters.ContainsKey(boosterTypeEnum))
                _activeBoosters[boosterTypeEnum].SwitchBoosterOn();

        }

        public Booster CreateNewBooster(BoosterTypeEnum boosterTypeEnum)
        {
            Booster result = null;
            switch (boosterTypeEnum)
            {
                case BoosterTypeEnum.AttackSpeed:
                    result= new AttackSpeed(this, _timerService, _windowManager, _unitsBuilder, _playerUnitsBuilderTwo);
                    break;
                case BoosterTypeEnum.AutoMerge:
                    result = new AutoMerge(this, _timerService, _windowManager,_mergeController);
                    break;
                case BoosterTypeEnum.GainCoins:
                    result =  new GainMoney(this, _timerService, _windowManager);
                    break;
                
            }

            return result;
        }

        public void CoroutineStart(IEnumerator enumerator)
        {
            StartCoroutine(enumerator);
        }

        public void CoroutineStop(IEnumerator enumerator)
        {
            StopCoroutine(enumerator);
        }
        
    }
}