using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Tools;
using UI.Content.Rewards;
using UI.UIManager;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using Random = UnityEngine.Random;

namespace UI.Windows
{
    public class FloatingCollectibleWindow : Window
    {
        private const int MULTUPLAY = 6;
        [SerializeField] private FloatingCollectibleElement _movableTransform;
        [SerializeField] private float _buttonUpDuration;
        [SerializeField] private float _buttonUpStayDuration;
        [SerializeField] private float _buttonDownDuration;
        
        [Inject] private readonly IWindowManager _windowManager;
        
        private Queue<FloatingCollectibleElement> poolMoveble = new Queue<FloatingCollectibleElement>();
        private List<BackupDestinationButton> movableContainers = new List<BackupDestinationButton>();
        private Dictionary<int, BackupDestinationButton.OriginButton> originButtons = new Dictionary<int, BackupDestinationButton.OriginButton>();
        private Dictionary<int, bool> finalAnimationStarted = new Dictionary<int, bool>();
        
        public UnityAction AfterCompleteAnimations { private get; set; }
        
        private int nextSequenceCollection = 100;

        protected override void Awake()
        {
            base.Awake();
            poolMoveble.Enqueue(_movableTransform);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            originButtons.Clear();
            finalAnimationStarted.Clear();

            AfterCompleteAnimations?.Invoke();
        }

        public void ShowSafeNoAnimation()
        {
            _manager.Show(this, _manager.WindowTransitions.InstantFadeWithoutBlock, WindowPriority.AboveTopPanel);
            _movableTransform.gameObject.SetActive(false);
        }
        
        public void SetCollectible(IEnumerable<Collectible> collectibles, Transform startTransform)
        {
            int elementsCount = collectibles.Sum(c => GetMaxElementCount(c.Type));
        
            if (elementsCount == 0)
            {
                _manager.GetWindow<TopPanelWindow>().SkipProfileChangedAnimation();
                AfterCompleteAnimations?.Invoke();
                return;
            }
        
            if (elementsCount == 1)
            {
                SetCollectible(collectibles.First(), startTransform);
                return;
            }
        
            int step = 360 / elementsCount;
        
            _manager.Show(this, _windowManager.WindowTransitions.InstantFadeWithoutBlock, WindowPriority.AboveTopPanel);
        
            int circleIndex = 1;
            foreach (var collectible in collectibles)
            {
                int max = GetMaxElementCount(collectible.Type);
                int realElementsCount = Mathf.Min(max, collectible.Amount);
                for (int i = 0; i < realElementsCount; i++)
                    SetCollectibleInternal(collectible, startTransform, step * circleIndex, nextSequenceCollection);
            }
            nextSequenceCollection++;
        }
        
        public void SetCollectible(Collectible collectible, Transform startTransform)
        {
            _manager.Show(this, _windowManager.WindowTransitions.InstantFadeWithoutBlock, WindowPriority.AboveTopPanel);
        
            SetCollectibleInternal(collectible, startTransform, -1, nextSequenceCollection);
            nextSequenceCollection++;
        }
        
         private void SetCollectibleInternal(Collectible collectible, Transform startTransform, int circleStep, int sequenceCollection)
        {
            if (collectible == null) return;
            
            var rewardType = collectible.Type;
            
            BackupDestinationButton backupDestinationButton = GetDestinationButton(rewardType, sequenceCollection);
            DestinationButtonAnimateStart(backupDestinationButton);
            
            FloatingCollectibleElement movable;
            if (poolMoveble.Count == 0)
                movable = Instantiate(_movableTransform, transform, false);
            else
                movable = poolMoveble.Dequeue();
            
            backupDestinationButton.Elements.Add(movable);
            backupDestinationButton.ElementsArrivedCount++;
            
            var circleStart = Vector3.zero;
            if (circleStep >= 0)
            {
                var q = Quaternion.Euler(0, 0, circleStep + Rand.Logistic(0, 4));
                circleStart = q * new Vector3(0, -125 + Random.Range(0, 50), 0);
            }
            
            movable.SetCollectible(collectible, startTransform.position,
                startTransform.position + circleStart,
                backupDestinationButton.Buttons.Destination != null ? backupDestinationButton.Buttons.Destination.position : backupDestinationButton.Buttons.Origin.transform.position);
            
            movable.DestinationArrived += DestinationArrivedHandler;
            movable.AnimationComplete += AnimationCompleteHandler;
        }
         
        private BackupDestinationButton GetDestinationButton(RewardType rewardType, int sequenceCollection)
        {
            var backupDestinationButton = movableContainers.Find(c => c.RewardType == rewardType && c.CollectionHash == sequenceCollection);

            if (backupDestinationButton == null)
            {
                TopPanelWindow topPanel = _manager.GetWindow<TopPanelWindow>();
                BottomPanelWindow bottomPanelWindow = _manager.GetWindow<BottomPanelWindow>();

                BackupDestinationButton.OriginButton originButton;
                switch (rewardType)
                {
                    case RewardType.Coin:
                        originButton = GetOriginButton(bottomPanelWindow.WalletOrigin.gameObject);
                       // originButton = GetOriginButton(topPanel.CashLabelTransform.gameObject);
                        break;
                    case RewardType.Gem:
                        originButton = GetOriginButton(bottomPanelWindow.WalletOrigin.gameObject);
                        //originButton = GetOriginButton(topPanel.GoldLabelTransform.gameObject);
                        break;
                    case RewardType.AttackSpeed:
                    case RewardType.GainCoins:
                    case RewardType.AutoMerge:
                        originButton = GetOriginButton(bottomPanelWindow.FortuneWheelBtn.gameObject);
                        break;
                    default:
                        originButton = GetOriginButton(topPanel.RespectLabelTransform.gameObject);
                        break;
                }

                backupDestinationButton = new BackupDestinationButton(rewardType)
                {
                    Buttons = originButton
                };


                backupDestinationButton.CollectionHash = sequenceCollection;
                backupDestinationButton.ElementsArrivedCount = 0;

                movableContainers.Add(backupDestinationButton);
            }

            return backupDestinationButton;
        }
        
        private BackupDestinationButton.OriginButton GetOriginButton(GameObject origin, Transform destination = null)
        {
            BackupDestinationButton.OriginButton originButton;
            if (!originButtons.TryGetValue(origin.GetInstanceID(), out originButton))
            {
                originButton = new BackupDestinationButton.OriginButton()
                {
                    Origin = origin,
                    Destination = destination
                };
                originButtons.Add(origin.GetInstanceID(), originButton);

                if (originButton.Destination != null)
                {
                    originButton.OriginWasEnable = originButton.Origin.activeInHierarchy;
                }
            }
            return originButton;
        }

                private void DestinationButtonAnimateStart(BackupDestinationButton backupDestinationButton)
        {
            if (backupDestinationButton.Buttons.Destination != null)
            {
                if (backupDestinationButton.Buttons.OriginWasEnable)
                {
                    backupDestinationButton.Buttons.Origin.SetActive(false);
                }

                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)backupDestinationButton.Buttons.Destination.parent.parent);
                backupDestinationButton.Buttons.Destination.gameObject.SetActive(true);
            }
        }

        private void DestinationButtonAnimFinish(BackupDestinationButton backupDestinationButton)
        {
            Sequence destinationAnim = DOTween.Sequence();
            destinationAnim.timeScale = Time.timeScale;
            
            if (backupDestinationButton.Buttons.Destination != null)
            {
                destinationAnim.Append(backupDestinationButton.Buttons.Destination.DOScale(1.4f, _buttonUpDuration));
                destinationAnim.AppendInterval(_buttonUpStayDuration);
                destinationAnim.Append(backupDestinationButton.Buttons.Destination.DOScale(1, _buttonDownDuration));

                destinationAnim.AppendCallback(() =>
                {
                    if (backupDestinationButton.Buttons.OriginWasEnable)
                    {
                        backupDestinationButton.Buttons.Origin.SetActive(true);
                    }

                    backupDestinationButton.Buttons.Destination.gameObject.SetActive(false);
                });
            }
            else
            {
                destinationAnim.Append(backupDestinationButton.Buttons.Origin.transform.DOScale(1.4f, _buttonUpDuration));
                destinationAnim.AppendInterval(_buttonUpStayDuration);
                destinationAnim.Append(backupDestinationButton.Buttons.Origin.transform.DOScale(1, _buttonDownDuration));
            }

            backupDestinationButton.Sequence = destinationAnim;
        }

        private void DestinationAnimation(BackupDestinationButton backupDestinationButton)
        {
            if (backupDestinationButton.RewardType == RewardType.Coin
                || backupDestinationButton.RewardType == RewardType.Gem
                || backupDestinationButton.RewardType == RewardType.AttackSpeed
                || backupDestinationButton.RewardType == RewardType.AutoMerge
                || backupDestinationButton.RewardType == RewardType.GainCoins
                )
            {
                bool isFinalAnimationStarted = finalAnimationStarted.ContainsKey(backupDestinationButton.CollectionHash);
                if (!isFinalAnimationStarted)
                {
                    _manager.GetWindow<TopPanelWindow>().PlayProfileChangedAnimation();
                    finalAnimationStarted[backupDestinationButton.CollectionHash] = true;
                }

            } 
            // else if (backupDestinationButton.RewardType == RewardType.ItemRare
            //     || backupDestinationButton.RewardType == RewardType.Truck
            //     )
            // {
            //     DestinationButtonAnimFinish(backupDestinationButton);
            // }
        }
        
        public void DestinationArrivedHandler(FloatingCollectibleElement element)
        {
            element.DestinationArrived -= DestinationArrivedHandler;
            var destinationButton = movableContainers.Find(c => c.Elements.Contains(element));
            DestinationAnimation(destinationButton);

            destinationButton.ElementsArrivedCount--;

            if (movableContainers
                .Where(c => c.CollectionHash == destinationButton.CollectionHash)
                .Sum(c => c.ElementsArrivedCount) == 0
                    && !finalAnimationStarted.ContainsKey(destinationButton.CollectionHash))
            {
                _manager.GetWindow<TopPanelWindow>().SkipProfileChangedAnimation();
            }
        }

        public void AnimationCompleteHandler(FloatingCollectibleElement element)
        {
            element.AnimationComplete -= AnimationCompleteHandler;

            var destinationButton = movableContainers.Find(c => c.Elements.Contains(element));
            destinationButton.Elements.Remove(element);
            poolMoveble.Enqueue(element);

            if (destinationButton.Elements.Count == 0)
            {
                movableContainers.Remove(destinationButton);
                DeactivateDestinationButton(destinationButton);
            }

            if (movableContainers.Count == 0)
            {
                _manager.Hide(this);
            }
        }

        private void DeactivateDestinationButton(BackupDestinationButton button)
        {
            if (button.Sequence != default && button.Sequence.active && !button.Sequence.IsComplete())
            {
                button.Sequence?.Goto(button.Sequence.Duration());
                button.Sequence?.Kill(true);
            }

            if (button.Buttons.Destination != default)
                button.Buttons.Destination.localScale = Vector3.one;
            if (button.Buttons.Origin != default)
                button.Buttons.Origin.transform.localScale = Vector3.one;
        }
        
        private int GetMaxElementCount(RewardType rewardType)
        {
            switch(rewardType)
            {
                case RewardType.Coin:
                case RewardType.Gem:
                case RewardType.AutoMerge:
                case RewardType.GainCoins:
                case RewardType.AttackSpeed:
                    return MULTUPLAY;
                default:
                    return 1;
            }
        }
        
        private class BackupDestinationButton
        {
            public class OriginButton
            {
                public GameObject Origin;
                public bool OriginWasEnable;
                public Transform Destination;
            }

            public readonly RewardType RewardType;
            public OriginButton Buttons;
            public Sequence Sequence;
            public int CollectionHash;

            public List<FloatingCollectibleElement> Elements = new List<FloatingCollectibleElement>();
            public int ElementsArrivedCount;

            public BackupDestinationButton(RewardType rewardType)
            {
                RewardType = rewardType;
            }
        }
    }
}