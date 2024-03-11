using Models.DataModels;
using TonkoGames.StateMachine;
using Tools.Debugs.Cheats;
using UnityEngine;
using VContainer;

namespace TonkoGames
{
    public class Admin : MonoBehaviour
    {

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private GameCheats _gameCheats;
        
        [Inject] private void Init(IDataCentralService dataCentralService, ICoreStateMachineCheat coreStateMachine)
        {
            _gameCheats = new GameCheats();
            _gameCheats.Init(dataCentralService, coreStateMachine);
        }
#else
        private void Awake()
        {
            gameObject.SetActive(false);
        }
#endif
    }
}