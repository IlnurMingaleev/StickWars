using System.ComponentModel;
using TonkoGames.StateMachine;
using Models.DataModels;
using UnityEditor;

namespace Tools.Debugs.Cheats.SRDebuggerOptions
{
    public class CommonCheatsPanel
    {
        private readonly IDataCentralService _dataCentralService;
        private readonly ICoreStateMachineCheat _coreStateMachineCheat;

        public CommonCheatsPanel(IDataCentralService dataCentralService, ICoreStateMachineCheat coreStateMachineCheat)
        {
            _dataCentralService = dataCentralService;
            _coreStateMachineCheat = coreStateMachineCheat;
        }

        [Category("Main")]
        public void ClearUser()
        {
            _dataCentralService.Restart();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
        

        [Category("Common")]
        [SROptions.IncrementAttribute(10)]
        public int Coins
        {
            get => _dataCentralService.StatsDataModel.CoinsCount.Value;
            set => _dataCentralService.StatsDataModel.SetCoinsCount(value);
        }
        
        [Category("Common")]
        [SROptions.IncrementAttribute(10)]
        public int Gems
        {
            get => _dataCentralService.StatsDataModel.GemsCount.Value;
            set => _dataCentralService.StatsDataModel.SetGemsCount(value);
        }
        
        [Category("Battle")]
        public void Battle_Finish()
        {
            _coreStateMachineCheat.BattleStateMachineCheat.CheatEndBattle(true);
            SRDebug.Instance.HideDebugPanel();
        }
    }
}