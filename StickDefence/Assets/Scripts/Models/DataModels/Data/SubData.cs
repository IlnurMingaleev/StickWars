using System;
using Enums;
using TonkoGames.StateMachine.Enums;

namespace Models.DataModels.Data
{
    [Serializable]
    public struct SubData
    {
        public TutorialStepsEnum LuckySpinTutorialStep;
        
        public bool IsADSRemove;
        
        public float SoundVolume;
        public float MusicVolume;
    }
}