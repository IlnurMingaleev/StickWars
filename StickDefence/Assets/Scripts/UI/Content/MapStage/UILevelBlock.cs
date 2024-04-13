using DG.Tweening;
using Enums;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.Controllers;
using Models.DataModels;
using Models.IAP;
using Models.Player;
using UI.Common;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.MapStage
{
    public class UILevelBlock : UIBehaviour
    {
        [Header("Visual")]
        [SerializeField] private GameObject _focusGO;
        [SerializeField] private DOTweenAnimation _tweenOpenFadeBlock;
        [SerializeField] private CanvasGroup _canvasGroupBlock;
        
        [Header("Core")]
        [SerializeField] private MapStagesEnum _mapStageType;
        [SerializeField] private MapStagesEnum _previousLevelType;
        [SerializeField] private UIButton _buttonSelectLevel;
        
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly IIAPService _iapService;
        [Inject] private readonly IWindowManager _windowManager;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly IPlayer _player;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetupStage();
        }

        private void SetupStage()
        {
            var levelData = _dataCentralService.MapStageDataModel.GetMapStageData(_mapStageType);

            bool isOpened = false;
            if (_previousLevelType == MapStagesEnum.None)
            {
                isOpened = true;
            }
            else
            {
                isOpened = _dataCentralService.MapStageDataModel.GetMapStageData(_previousLevelType).IsCompleted;
            }
            
            if (isOpened)
            {
                if (levelData.IsAnimationOpened)
                {
                    _canvasGroupBlock.alpha = 0;
                }
                else
                {
                    _tweenOpenFadeBlock.CreateTween();
                    _tweenOpenFadeBlock.DOPlay();
                    levelData.IsAnimationOpened = true;
                    _dataCentralService.MapStageDataModel.UpdateMapStageData(levelData);
                    _dataCentralService.SaveFull();
                }
                
                _focusGO.SetActive(levelData.IsCompleted);
                _buttonSelectLevel.IsInteractable = true;
                _buttonSelectLevel.OnClickAsObservable.TakeUntilDisable(this).Subscribe(_ => OnSelectLevel());
            }
            else
            {
                _focusGO.SetActive(false);
                _buttonSelectLevel.IsInteractable = false;
                _canvasGroupBlock.alpha = 1;
            }
        }

        private void OnSelectLevel()
        {
            var isShowing = _iapService.ShowCommercialBreak();

            if (!isShowing)
            {
                _windowManager.Show<FadeWindow>().CloseFade(EndStartBattleFade);
            }
        }

        private void EndStartBattleFade()
        {
            _coreStateMachine.GameStateMachine.SetGameState(GameStateEnum.StageBattle);
            _coreStateMachine.BattleStateMachine.SetBattleState(BattleStateEnum.LoadBattle);
            _windowManager.Hide<BattleSelectionMapWindow>();
        }
    }
}