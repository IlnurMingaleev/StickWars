using Models.Timers;
using TonkoGames.Sound;
using Views.Units.Units;

namespace Models.Units.Units
{
    public class BasePlayerUnitUnitTwo: BasePlayerUnit
    {
        public BasePlayerUnitUnitTwo(UnitView unitView, ITimerService timerService, ISoundManager soundManager) : base(unitView, timerService, soundManager)
        {
        }
    }
}