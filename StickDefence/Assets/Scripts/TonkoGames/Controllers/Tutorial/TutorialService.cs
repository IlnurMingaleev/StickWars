using Enums;
using Models.DataModels;
using TonkoGames.Analytics;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;
using UniRx;

namespace TonkoGames.Controllers.Tutorial
{
    public class TutorialService
    {
        private ICoreStateMachine _coreStateMachine;
        private IDataCentralService _dataCentralService;
        private IWindowManager _windowManager;
        private ConfigManager _configManager;

        private CompositeDisposable _disposables = new CompositeDisposable();
        
        private DialogWindow _dialogWindow = null;
        private TutorialWindow _tutorialWindow = null;

        public void Init(ICoreStateMachine coreStateMachine, IDataCentralService dataCentralService, IWindowManager windowManager, ConfigManager configManager)
        {
            _coreStateMachine = coreStateMachine;
            _dataCentralService = dataCentralService;
            _windowManager = windowManager;
            _configManager = configManager;
            
            _tutorialWindow = _windowManager.GetWindow<TutorialWindow>();
            _tutorialWindow.Priority = WindowPriority.Tutorial;
            _dialogWindow = _windowManager.GetWindow<DialogWindow>();
            _dialogWindow.Priority = WindowPriority.DialogTutorial;
            
            //_coreStateMachine.TutorialStateMachine.LuckySpinTutorialStep.Subscribe(LuckySpinTutorialStateSwitch).AddTo(_disposables);
        }
   
        private void LuckySpinTutorialStateSwitch(TutorialStepsEnum tutorialStepsEnum)
        {
            switch (tutorialStepsEnum)
            {
                case TutorialStepsEnum.LuckySpinFirst:
                    _windowManager.Show(_tutorialWindow);
                    _tutorialWindow.SetFadeNormal();
                    break;
                case TutorialStepsEnum.LuckySpinDialog:
                    _windowManager.Show(_tutorialWindow);
                    _tutorialWindow.SetFadeNormal();
                    _windowManager.Show(_dialogWindow);
                    _dialogWindow.ShowDialog(tutorialStepsEnum.ToTranslatedName(), LuckySpinDialogNext, AnchorsEnum.RightDown);
                    break;
                case TutorialStepsEnum.End:
                    _windowManager.Hide(_tutorialWindow);
                    _windowManager.Hide(_dialogWindow);
                    _dataCentralService.SubData.SetLuckySpinTutorialStep(TutorialStepsEnum.End);
                    _dataCentralService.SaveFull();
                    break;
            }
            GameAnalytics.Instance.PushEvent(StringsHelper.Analytics.mapStage_tutorial, tutorialStepsEnum.ToString());
        }
        
   
        private void LuckySpinDialogNext()
        {
            _coreStateMachine.TutorialStateMachine.NextLuckySpinTutorialStep();
        }

        ~TutorialService()
        {
            _disposables.Clear();
        }
    }
}