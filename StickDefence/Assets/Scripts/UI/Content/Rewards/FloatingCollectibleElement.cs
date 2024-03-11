using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using DG.Tweening.Plugins.Core.PathCore;
using UI.Content.Rewards;

namespace UI.Content.Rewards
{
    public class FloatingCollectibleElement : MonoBehaviour
    {
        [SerializeField] private UICollectible _uiCollectible;
        [SerializeField] private GameObject _coinCoin;
        [SerializeField] private GameObject _gemCoin;
        [SerializeField] private Transform _scalableTransform;
        [SerializeField] private float _moveDelaySeconds;
        [SerializeField] private float _moveDurationSeconds;
        [SerializeField] private Ease _moveAnimationEase;
        [SerializeField] private Ease _scaleAnimationEase;
        [SerializeField] private DOTweenAnimation _finalAnimation;
        [SerializeField][Range(0, 0.5f)] private float _waveAmplitude;
        
        private Sequence _moveAndScaleAnimation;

        private RewardType rewardType;
        public RewardType RewardType => rewardType;

        public event Action<FloatingCollectibleElement> AnimationComplete;
        public event Action<FloatingCollectibleElement> DestinationArrived;

        public void SetCollectible(Collectible collectible, Vector3 start, Vector3 circleStart, Vector3 finish)
        {
            if (collectible == null) return;

            Transform scalableTransform = null;

            if (collectible.Type is RewardType.Coin or RewardType.Gem)
            {
                _uiCollectible.gameObject.SetActive(false);
                if (collectible.Type == RewardType.Coin)
                {
                    scalableTransform = _coinCoin.transform;
                    SetupActiveCoins(isCoinCoin: true);
                }
                else if (collectible.Type == RewardType.Gem)
                {
                    scalableTransform = _gemCoin.transform;
                    SetupActiveCoins(isGemCoin: true);
                }
            }
            else
            {
                _uiCollectible.UpdateCollectible(collectible);
                _uiCollectible.gameObject.SetActive(true);
                SetupActiveCoins();

                scalableTransform = _scalableTransform;
            }
            rewardType = collectible.Type;
            Setup(scalableTransform, collectible.Type, start, circleStart, finish);
        }

        private void SetupActiveCoins(bool isCoinCoin = false, bool isGemCoin = false)
        {
            _coinCoin.SetActive(isCoinCoin);
            _gemCoin.SetActive(isGemCoin);
        }
        private void Setup(Transform scalableTransform, RewardType rewardType, Vector3 start, Vector3 circleStart, Vector3 finish)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            transform.position = start;

            _moveAndScaleAnimation?.Complete(true);

            _moveAndScaleAnimation = DOTween.Sequence();

            _moveAndScaleAnimation.timeScale = 1;
            _moveAndScaleAnimation.Append(
                transform.DOMove(circleStart, _moveDelaySeconds)
                    .SetEase(Ease.OutQuad)
                );

            _moveAndScaleAnimation.AppendInterval(0.2f);

            _moveAndScaleAnimation.Append(
                transform
                    .DOPath(GeneratePath(circleStart, finish), _moveDurationSeconds)
                    .SetEase(_moveAnimationEase)
                );
            
            _moveAndScaleAnimation.Join(
                scalableTransform
                    .DOScale(Vector3.zero, _moveDurationSeconds)
                    .From(Vector3.one)
                    .SetEase(_scaleAnimationEase)
            );

            _moveAndScaleAnimation.OnComplete(StartShineAnimations);
        }

        private void OnDisable()
        {
            if (_moveAndScaleAnimation != null && _moveAndScaleAnimation.IsActive())
            {
                _moveAndScaleAnimation.Kill(false);
                _moveAndScaleAnimation = null;
            }
        }

        private void StartShineAnimations()
        {
            DestinationArrived?.Invoke(this);

            if (_finalAnimation == null)
                OnCompleteAnimations();
            else
                _finalAnimation.tween.OnComplete(OnCompleteAnimations);
        }

        private void OnCompleteAnimations()
        {
          //  if (_finalAnimation == null) _finalAnimation.tween.Rewind();
            gameObject.SetActive(false);
            AnimationComplete?.Invoke(this);
        }

        private Path GeneratePath(Vector3 start, Vector3 finish)
        {
            var points = new List<Vector3>();

            var dir = finish - start;
            var len = dir.magnitude;

            var relUp = Vector3.Cross(dir.normalized, Vector3.forward).normalized;

            points.Add(Vector3.Lerp(start, finish, UnityEngine.Random.Range(0.15f, 0.35f)) + relUp * _waveAmplitude * len * UnityEngine.Random.Range(-0.5f, 0.5f));
            points.Add(finish);

            return new Path(PathType.CatmullRom, points.ToArray(), 10);
        }
    }
}