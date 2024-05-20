using Cysharp.Threading.Tasks;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.Battle;
using TimeSpan = System.TimeSpan;

namespace Anim.Battle
{
    public class BattleAnimations
    {
        private const int SecondsWaitAfterLaunchAnim = 1;
        
        private readonly PlayerFortressInstantiate _playerFortressInstantiate;
        private readonly ICoreStateMachine _coreStateMachine;
        
        public BattleAnimations(PlayerFortressInstantiate playerFortressInstantiate, ICoreStateMachine coreStateMachine)
        {
            _playerFortressInstantiate = playerFortressInstantiate;
            _coreStateMachine = coreStateMachine;
        }
        
        public async UniTaskVoid LaunchAnimation()
        {
         //  _playerFortressInstantiate.StartLaunchAnim();
       //     await UniTask.WaitUntil(() => _playerFortressInstantiate.IsLaunchIsProgress);
            await UniTask.Delay(TimeSpan.FromSeconds(SecondsWaitAfterLaunchAnim), ignoreTimeScale: false);
            _coreStateMachine.BattleStateMachine.SetBattleState(BattleStateEnum.StartBattle);
        }
    }
}