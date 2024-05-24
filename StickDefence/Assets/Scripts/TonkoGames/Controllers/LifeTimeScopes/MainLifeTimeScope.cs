using Models.Controllers;
using Models.DataModels;
using Models.Fabrics;
using Models.GameRewards;
using Models.IAP;
using Models.Player;
using Models.Timers;
using TonkoGames.Controllers.Core;
using TonkoGames.MultiScene;
using TonkoGames.Sound;
using TonkoGames.StateMachine;
using Tools.GameTools;
using UI.UIManager;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TonkoGames.Controllers.LifeTimeScopes
{
    public class MainLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(Object.FindObjectOfType<SoundManager>()).As<ISoundManager>();
            builder.RegisterComponent(Object.FindObjectOfType<WindowManager>()).As<IWindowManager>();
            builder.RegisterComponent(Object.FindObjectOfType<ConfigManager>()).As<ConfigManager>();
            builder.RegisterComponent(Object.FindObjectOfType<CoroutineTimer>()).As<CoroutineTimer>();

            builder.RegisterEntryPoint<ScenesControllerModel>();
            
            builder.Register<TimerService>(Lifetime.Singleton).As<ITimerService>();
            builder.Register<PrefabInject>(Lifetime.Singleton);
            builder.Register<CoreStateMachine>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<Player>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<IMultiSceneManager, MultiSceneManager>(Lifetime.Singleton);
            builder.Register<GameRewardsService>(Lifetime.Singleton);
            builder.Register<DataCentralService>(Lifetime.Singleton).As<IDataCentralService, DataCentralService>();
            builder.Register<LobbyModels>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<IAPService>(Lifetime.Singleton).As<IIAPService, IAPService>();
        }
    }
}