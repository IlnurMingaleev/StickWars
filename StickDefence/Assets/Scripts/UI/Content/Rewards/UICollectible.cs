using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI.Content.Rewards
{
    public class UICollectible : MonoBehaviour
    {
        [SerializeField] private DynamicSpriteCurrency _collectibleCoin;
        [SerializeField] private DynamicSpriteCurrency _collectibleGem;
        [SerializeField] private DynamicSpriteBooster _collectibleAttackSpeed;
        [SerializeField] private DynamicSpriteBooster _collectibleGainCoins;
        [SerializeField] private DynamicSpriteBooster _collectibleAutoMerge;
        [SerializeField] private float _collectibleDownDuration = default;
        [SerializeField] private float _delayDownValue = default;
        
        private Sequence _sequenceAnim;
        private List<Transform> _collectiblesAnim = new List<Transform>();

        private void Awake()
        {
            DeactivateBlocks();
        }

        private void OnDisable()
        {
            DeactivateAnim();
            DeactivateBlocks();
        }
        
        public void UpdateCollectible(Collectible collectible)
        {
            switch (collectible.Type)
            {
                case RewardType.Coin:
                    _collectibleCoin.UpdateSprite(collectible.Amount);
                    _collectibleCoin.UpdateValue(collectible.Amount);
                    break;
                case RewardType.Gem:
                    _collectibleGem.UpdateSprite(collectible.Amount);
                    _collectibleGem.UpdateValue(collectible.Amount);
                    break;
                case RewardType.AttackSpeed:
                    _collectibleAttackSpeed.UpdateValue(collectible.Amount);
                    break;
                case RewardType.GainCoins:
                    _collectibleGainCoins.UpdateValue(collectible.Amount);
                    break;
                case RewardType.AutoMerge:
                    _collectibleAutoMerge.UpdateValue(collectible.Amount);
                    break;


            }

            _collectibleCoin.gameObject.SetActive(collectible.Type == RewardType.Coin);
            _collectibleGem.gameObject.SetActive(collectible.Type == RewardType.Gem);
            _collectibleAttackSpeed.gameObject.SetActive(collectible.Type == RewardType.AttackSpeed);
            _collectibleGainCoins.gameObject.SetActive(collectible.Type == RewardType.GainCoins);
            _collectibleAutoMerge.gameObject.SetActive(collectible.Type == RewardType.AutoMerge);
        }
        
        private void UpdateLocalCollectible(Collectible collectible)
        {
            switch (collectible.Type)
            {
                case RewardType.Coin:
                    _collectibleCoin.UpdateValue(collectible.Amount);
                    _collectiblesAnim.Add(_collectibleCoin.transform);
                    break;
                case RewardType.Gem:
                    _collectibleGem.UpdateValue(collectible.Amount);
                    _collectiblesAnim.Add(_collectibleGem.transform);
                    break;
                case RewardType.AttackSpeed:
                    _collectibleAttackSpeed.UpdateValue(collectible.Amount);
                    _collectiblesAnim.Add(_collectibleAttackSpeed.transform);
                    break;
                case RewardType.GainCoins:
                    _collectibleGainCoins.UpdateValue(collectible.Amount);
                    _collectiblesAnim.Add(_collectibleGainCoins.transform);
                    break;
                case RewardType.AutoMerge:
                    _collectibleAutoMerge.UpdateValue(collectible.Amount);
                    _collectiblesAnim.Add(_collectibleAutoMerge.transform);
                    break;
            }
        }

        public void UpdateCollectibleAnimation(List<Collectible> fullCollectibles)
        {
            _collectiblesAnim.Clear();
            _sequenceAnim = DOTween.Sequence();

            foreach (var fullCollectible in fullCollectibles)
            {
                UpdateLocalCollectible(fullCollectible);
            }
            
            foreach (var collectibleAnim in _collectiblesAnim)
            {
                collectibleAnim.localScale = new Vector3(0, 0, 0);
                _sequenceAnim.Append(collectibleAnim.DOScale(1, _collectibleDownDuration)
                    .SetDelay(_delayDownValue)
                    .SetEase(Ease.InQuart)
                    .OnStart(() => collectibleAnim.gameObject.SetActive(true)));
            }
            
            _sequenceAnim.AppendCallback(() =>
            {
                _sequenceAnim = null;
            });
        }
        
        private void DeactivateAnim()
        {
            if (_sequenceAnim != null && _sequenceAnim.IsActive())
            {
                _sequenceAnim.Kill(false);
                _sequenceAnim = null;
            }
        }

        private void DeactivateBlocks()
        {
            _collectibleCoin.gameObject.SetActive(false);
            _collectibleGem.gameObject.SetActive(false);
            _collectibleAttackSpeed.gameObject.SetActive(false);
            _collectibleAutoMerge.gameObject.SetActive(false);
            _collectibleGainCoins.gameObject.SetActive(false);
        }
    }
}