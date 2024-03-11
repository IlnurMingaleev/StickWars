using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Enums;
using UI.Common;
using UI.Content.Rewards;
using UI.UIManager;
using UniRx;
using UnityEngine;

namespace UI.Windows
{
    public class RewardCollectWindow : Window
    {
        [SerializeField] private UIButton _collectButton;
        [SerializeField] private UICollectible _uiCollectible;
        public override WindowPriority Priority { get; set; } = WindowPriority.AboveTopPanel;
        
        private readonly List<Collectible> _fullCollectibles = new List<Collectible>();
        private readonly List<Collectible> _floatingCollectibles = new List<Collectible>();
        
        private bool _needToShowFloatingAnimation = true;
        public bool IsFloatingCollectiblesInAnimation { get; private set; }
        public bool IsShowInProgress { get; private set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            _collectButton.gameObject.SetActive(true);
            _collectButton.OnClickAsObservable.Subscribe(_ => OnFinalClickCollect()).AddTo(ActivateDisposables);
        }
        
        public async UniTask Collect(RewardContains rewardContains, bool needToShowFloatingAnimation = true)
        {
            _needToShowFloatingAnimation = needToShowFloatingAnimation;
            _floatingCollectibles.Clear();
            _fullCollectibles.Clear();
            
            _manager.GetWindow<TopPanelWindow>().SetIgnoreNextProfileChange();
            
            if (rewardContains.Coin > 0)
            {
                _floatingCollectibles.Add(new CollectibleCoin(rewardContains.Coin));
                _fullCollectibles.Add(_floatingCollectibles.Last());
            }
            if (rewardContains.Gem > 0)
            {
                _floatingCollectibles.Add(new CollectibleGem(rewardContains.Gem));
                _fullCollectibles.Add(_floatingCollectibles.Last());
            }
            
            _uiCollectible.UpdateCollectibleAnimation(_fullCollectibles);
            
            IsShowInProgress = true;
            await UniTask.WaitWhile(() => IsShowInProgress);
        }
        
        private void OnFinalClickCollect()
        {
            if (_needToShowFloatingAnimation && _floatingCollectibles.Any())
            {
                IsFloatingCollectiblesInAnimation = true;
                FloatingCollectibleWindow collectibleWindow = _manager.GetWindow<FloatingCollectibleWindow>();
                collectibleWindow.AfterCompleteAnimations = () => IsFloatingCollectiblesInAnimation = false;
                collectibleWindow.SetCollectible(_floatingCollectibles, _uiCollectible.transform);
            }
            else
            {
                _manager.FindWindow<TopPanelWindow>()?.SkipProfileChangedAnimation();
                IsFloatingCollectiblesInAnimation = false;
            }
            
            _fullCollectibles.Clear();
            _floatingCollectibles.Clear();
            _collectButton.gameObject.SetActive(false);
            
            IsShowInProgress = false;

            _manager.Hide(this);
        }
    }
}