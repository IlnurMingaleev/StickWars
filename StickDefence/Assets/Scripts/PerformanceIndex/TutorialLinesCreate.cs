using System.Collections.Generic;
using Enums;
using TonkoGames.StateMachine.Enums;
using UnityEngine;

namespace PerfomanceIndex
{
    public static class TutorialLinesCreate
    {
        public static void CreateLuckySpinLine(this List<TutorialStepsEnum> tutorial)
        {
            tutorial.Add(TutorialStepsEnum.NoneStart);
            tutorial.Add(TutorialStepsEnum.LuckySpinFirst);
            tutorial.Add(TutorialStepsEnum.LuckySpinDialog);
            tutorial.Add(TutorialStepsEnum.End);
        }
    }
}