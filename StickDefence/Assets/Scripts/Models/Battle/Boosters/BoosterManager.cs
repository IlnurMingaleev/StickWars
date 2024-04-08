using System.Collections.Generic;
using Enums;
using Models.Timers;
using UI.UIManager;
using UI.Windows;
using UnityEngine;
using VContainer;

namespace Models.Battle.Boosters
{
    public class BoosterManager: MonoBehaviour
    {
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
            
        }

        public Booster CreateNewBooster(BoosterTypeEnum boosterTypeEnum)
        {
            Booster result = null;
            switch (boosterTypeEnum)
            {
                case BoosterTypeEnum.AttackSpeed:
                    result= new AttackSpeed(this, _timerService, _windowManager);
                    break;
                case BoosterTypeEnum.AutoMerge:
                    result = new AutoMerge(this, _timerService, _windowManager);
                    break;
                case BoosterTypeEnum.GainCoins:
                    result =  new GainMoney(this, _timerService, _windowManager);
                    break;
                
            }

            return result;
        }

    }
}