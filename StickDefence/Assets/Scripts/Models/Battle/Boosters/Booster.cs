using Enums;
using Models.Timers;
using VContainer;

namespace Models.Battle.Boosters
{
    public class Booster
    {
        protected const float _boosterActiveTime = 60f;
        protected BoosterTypeEnum _boosterType;
        protected ITimerModel _timerModel;


        public void StartTimer()
        {
            
        }

    }
}