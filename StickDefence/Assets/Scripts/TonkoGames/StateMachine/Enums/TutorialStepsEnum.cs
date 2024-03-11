using I2.Loc;
using PerfomanceIndex;

namespace TonkoGames.StateMachine.Enums
{
    public enum TutorialStepsEnum
    {
        NoneStart,
        End,
        Waiting,
        LuckySpinFirst,
        LuckySpinDialog,
    }

    public enum TutorialActionStateEnum
    {
        None,
        Progressing,
        End
    }

    public enum TutorialLineEnum
    {
        None,
        LuckySpin,
    }
    
    public static class TutorialStepsEnumExtensions
    {
        public static string ToTranslationTerm(this TutorialStepsEnum type)
        {
            return TutorialStep.TutorialStepNames[type];
        }
        
        public static string ToTranslatedName(this TutorialStepsEnum type)
        {
            return LocalizationManager.GetTranslation(type.ToTranslationTerm());
        }
    }
}