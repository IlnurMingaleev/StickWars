using System.Collections.Generic;
using Enums;
using TonkoGames.StateMachine.Enums;
using I2.Loc;

namespace PerfomanceIndex
{
    public class TutorialStep
    {
        public static Dictionary<TutorialStepsEnum, string> TutorialStepNames { get; } = new Dictionary<TutorialStepsEnum, string>()
        {
           // { TutorialStepsEnum.LuckySpinDialog, ScriptTerms.Dialogs_Tutorial_LuckySpin.LuckySpinDialog },
        };
    }
}