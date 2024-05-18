using System;
using System.Collections;
using Models.Merge;
using Models.Timers;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;
using UnityEngine;

namespace Models.Battle.Boosters
{
    public class AutoMerge: Booster
    {
        private MergeController _mergeController;
        private bool _shouldRun = false;
        public AutoMerge(BoosterManager boosterManager, CoroutineTimer boosterTimer, IWindowManager windowManager, MergeController mergeController) : base(boosterManager, boosterTimer, windowManager)
        {
            _mergeController = mergeController;
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
                yield return new WaitForSeconds(2.0f);
            }
        }
       


    }
}