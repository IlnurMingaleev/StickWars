using Models.Timers;
using TonkoGames.Sound;
using Views.Units.Units;

namespace Models.Units.Units
{
    public class PlayerUnitUnitTwo: PlayerUnit
    {
        public PlayerUnitUnitTwo(UnitView unitView, ITimerService timerService, ISoundManager soundManager) : base(unitView, timerService, soundManager)
        {
        }
    }
}