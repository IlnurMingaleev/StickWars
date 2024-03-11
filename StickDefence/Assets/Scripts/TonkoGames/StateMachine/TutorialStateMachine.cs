using System;
using System.Collections.Generic;
using Enums;
using Models.DataModels.Models;
using PerfomanceIndex;
using TonkoGames.StateMachine.Enums;
using UniRx;
using UnityEngine;

namespace TonkoGames.StateMachine
{
    public interface ITutorialStateMachine
    {
        #region LuckySpin

        IReadOnlyReactiveProperty<TutorialStepsEnum> LuckySpinTutorialStep { get; }
        bool IsLuckySpinTutorialShown { get; }

        void SetLuckySpinTutorialState(TutorialStepsEnum tutorialState);
        void NextLuckySpinTutorialStep();
        
        #endregion
        void NextStateBuyType(TutorialLineEnum tutorialLineEnum);
        bool IsTutorialShown(TutorialLineEnum tutorialLineEnum);
        IReadOnlyReactiveProperty<TutorialStepsEnum> GetTutorialStepReactive(TutorialLineEnum tutorialLineEnum);
        void InitTutorials(ISubDataModel subDataModel);
        bool IsAnyTutorialInProgressing();
    }
    public class TutorialStateMachine : ITutorialStateMachine
    {
        private ReactiveProperty<TutorialStepsEnum> _luckySpinTutorialStep = new ReactiveProperty<TutorialStepsEnum>();
        private ReactiveProperty<TutorialActionStateEnum> _luckySpinTutorialState = new ReactiveProperty<TutorialActionStateEnum>(TutorialActionStateEnum.None);
        private List<TutorialStepsEnum> _luckySpinTutorial = new List<TutorialStepsEnum>();
        private int _indexCurrentLuckySpinTutorial = 0;

        public IReadOnlyReactiveProperty<TutorialStepsEnum> LuckySpinTutorialStep => _luckySpinTutorialStep;
        public bool IsLuckySpinTutorialShown => _luckySpinTutorialState.Value == TutorialActionStateEnum.End;
        
        public TutorialStateMachine()
        {
            _luckySpinTutorial.CreateLuckySpinLine();
        }

        #region LuckySpin

        public void SetLuckySpinTutorialState(TutorialStepsEnum tutorialState)
        {
            _luckySpinTutorialStep.Value = tutorialState;

            _indexCurrentLuckySpinTutorial = _luckySpinTutorial.IndexOf(tutorialState);
            
            CheckLuckySpinTutorialFinish();
        }

        public void NextLuckySpinTutorialStep()
        {
            if (_indexCurrentLuckySpinTutorial < _luckySpinTutorial.Count - 1)
            {
                _indexCurrentLuckySpinTutorial++;
                _luckySpinTutorialStep.Value = _luckySpinTutorial[_indexCurrentLuckySpinTutorial];
            }

            CheckLuckySpinTutorialFinish();
        }

        private void CheckLuckySpinTutorialFinish()
        {
            if (_indexCurrentLuckySpinTutorial == 1)
            {
                _luckySpinTutorialState.Value = TutorialActionStateEnum.Progressing;
            }else if (_indexCurrentLuckySpinTutorial == _luckySpinTutorial.Count - 1)
            {
                _luckySpinTutorialState.Value = TutorialActionStateEnum.End;
            }
            else
            {                
                _luckySpinTutorialState.Value = TutorialActionStateEnum.Progressing;
            }
        }

        #endregion
        
        public void NextStateBuyType(TutorialLineEnum tutorialLineEnum)
        {
            switch (tutorialLineEnum)
            {
                case TutorialLineEnum.LuckySpin:
                    NextLuckySpinTutorialStep();
                    break;
            }
        }
        
        public void InitTutorials(ISubDataModel subDataModel)
        {
            SetLuckySpinTutorialState(subDataModel.LuckySpinTutorialStep.Value);
        }
        
        public bool IsTutorialShown(TutorialLineEnum tutorialLineEnum)
        {
            switch (tutorialLineEnum)
            {
                case TutorialLineEnum.LuckySpin:
                    return IsLuckySpinTutorialShown;
            }

            return true;
        }

        public IReadOnlyReactiveProperty<TutorialStepsEnum> GetTutorialStepReactive(TutorialLineEnum tutorialLineEnum)
        {
            switch (tutorialLineEnum)
            {
                case TutorialLineEnum.LuckySpin:
                    return _luckySpinTutorialStep;
            }

            return null;
        }

        public bool IsAnyTutorialInProgressing()
        {
            return _luckySpinTutorialState.Value == TutorialActionStateEnum.Progressing;
        }
    }
}