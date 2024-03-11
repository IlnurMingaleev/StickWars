using TonkoGames.StateMachine;
using Models.DataModels;
using Tools.Debugs.Cheats.SRDebuggerOptions;

namespace Tools.Debugs.Cheats
{
    public class GameCheatsPanel
    {
        private readonly CommonCheatsPanel OptionsContainer;

        public GameCheatsPanel(IDataCentralService dataCentralService, ICoreStateMachineCheat coreStateMachineCheat)
        {
            OptionsContainer = new CommonCheatsPanel(dataCentralService, coreStateMachineCheat);
        }
		
        public void Init()
        {
            SRDebug.Instance.AddOptionContainer(OptionsContainer);
        }

        public void Clear()
        {
            SRDebug.Instance.RemoveOptionContainer(OptionsContainer);
        }
    }
}