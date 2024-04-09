using System;
using System.Collections;
using Models.Merge;
using Models.Timers;
using UI.UIManager;
using UI.Windows;
using UnityEngine;

namespace Models.Battle.Boosters
{
    public class AutoMerge: Booster
    {
        private MergeController _mergeController;
        private bool _shouldRun = false;
        public AutoMerge(BoosterManager boosterManager, ITimerService timerService, IWindowManager windowManager, MergeController mergeController) : base(boosterManager, timerService, windowManager)
        {
            _mergeController = mergeController;
        }


        public override void ApplyBooster()
        {
            throw new NotImplementedException();
        }

        public override void SwitchBoosterOn()
        { 
            _shouldRun = true;
           _boosterManager.CoroutineStart(PerformAutoMerge());
        }

        public override void SwitchBoosterOff()
        {
            _shouldRun = false;
            _boosterManager.CoroutineStop(PerformAutoMerge());
        }
        
        IEnumerator PerformAutoMerge()
        {
            while (_shouldRun)
            {
                _mergeController.AutoMerge();
                yield return new WaitForSeconds(1.0f);
            }
        }
       


    }
}