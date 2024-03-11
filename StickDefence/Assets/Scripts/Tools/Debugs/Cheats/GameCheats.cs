using TonkoGames.StateMachine;
using Models.DataModels;

namespace Tools.Debugs.Cheats
{
    public class GameCheats
    {
        private GameCheatsPanel Panel;

        public void Init(IDataCentralService dataCentralService, ICoreStateMachineCheat coreStateMachine)
        {
            Panel = new GameCheatsPanel(dataCentralService, coreStateMachine);
            Panel.Init();
        }
    }
}