using System;
using DG.Tweening;
using Models.DataModels;
using UI.Content.Rewards;
using TMPro;
using Tools.Extensions;
using UI.Common;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI.Content.TopPanel
{
    public class FinanceBar : UIAbstractAnimProfileBar
    {
        protected class FinanceStack : ProfileStack
        {
            public long Coin;
            public long Gem;

            public long CoinNew = NEW_MARK_NODATA;
            public long GemNew = NEW_MARK_NODATA;

            public bool HasCoinNew => CoinNew != NEW_MARK_NODATA;
            public bool HasGemNew => GemNew != NEW_MARK_NODATA;

            public bool HasCoinModify => Coin < CoinNew;
            public bool HasGemModify => Gem < GemNew;

            public bool HasNewData => HasCoinNew || HasGemNew;
        }
        
        [SerializeField] private TextMeshProUGUI _coinLabel = default;
        [SerializeField] private TextMeshProUGUI _gemLabel = default;
        
        [SerializeField] private UIButton _coinButton = default;
        [SerializeField] private UIButton _gemButton = default;
        
        [SerializeField] private float _buttonUpDuration = default;
        [SerializeField] private float _textAnimDuration = default;
        [SerializeField] private float _buttonDownDuration = default;
        [SerializeField] private float _scaleValue = default;
                
        public Transform CoinLabelTransform => _coinLabel.transform;
        public Transform GemLabelTransform => _gemLabel.transform;
        
        private Action _actionGold;
        private Action _actionCash;

        [Inject] private readonly IDataCentralService _dataCentralService;
        
        public bool IsShopButtonVisible
        {
            set
            {
                _coinButton.gameObject.SetActive(value);
                _gemButton.gameObject.SetActive(value);
            }
        }
        
        protected override void OnEnable()
        {
            _dataCentralService.StatsDataModel.CoinsCount.Subscribe(value => _UpdateData()).AddTo(ActivateDisposables);
            _dataCentralService.StatsDataModel.GemsCount.Subscribe(value => _UpdateData()).AddTo(ActivateDisposables);
            
            _UpdateData();
            _gemButton.OnClickAsObservable.Subscribe(uib => _actionGold?.Invoke()).AddTo(ActivateDisposables);
            _coinButton.OnClickAsObservable.Subscribe(uib => _actionCash?.Invoke()).AddTo(ActivateDisposables);
        }
        
        public void SetAction(Action actionGold, Action actionCash)
        {
            _actionGold = actionGold;
            _actionCash = actionCash;
        }
        
        public override void PlayProfileChangedAnimation()
        {
            if (Stack.Count == 0)
                return;

            var profileStack = (FinanceStack)Stack.Dequeue();

            if (SequenceAnimIsValid())
            {
                sequenceAnim?.Kill(false);
                ResetAnimateInfluence();
            }

            sequenceAnim = DOTween.Sequence();
            sequenceAnim.timeScale = Time.timeScale;

            Sequence counterCashAnim = null;
            Sequence counterGoldAnim = null;

            if (profileStack.HasCoinNew)
            {
                if (profileStack.HasCoinModify)
                    counterCashAnim = CreateSequenceAnim(_coinLabel, profileStack.Coin, profileStack.CoinNew);
            }
            else if (profileStack.Coin < _dataCentralService.StatsDataModel.CoinsCount.Value && !profileStack.HasNewData)
            {
                counterCashAnim = CreateSequenceAnim(_coinLabel, profileStack.Coin, _dataCentralService.StatsDataModel.CoinsCount.Value);
            }
            else
            {
                _UpdateCoin(_dataCentralService.StatsDataModel.CoinsCount.Value);
            }

            if (profileStack.HasGemNew)
            {
                if (profileStack.HasGemModify)
                    counterGoldAnim = CreateSequenceAnim(_gemLabel, profileStack.Gem, profileStack.GemNew);
            }
            else if (profileStack.Gem < _dataCentralService.StatsDataModel.GemsCount.Value && !profileStack.HasNewData)
            {
                counterGoldAnim = CreateSequenceAnim(_gemLabel, profileStack.Gem, _dataCentralService.StatsDataModel.GemsCount.Value);
            }
            else
            {
                _UpdateGem(_dataCentralService.StatsDataModel.GemsCount.Value);
            }

            if (counterCashAnim != null)
                sequenceAnim.Append(counterCashAnim);

            if (counterGoldAnim != null)
            {
                if (counterCashAnim != null)
                    sequenceAnim.Join(counterGoldAnim);
                else
                    sequenceAnim.Append(counterGoldAnim);
            }

            sequenceAnim.AppendCallback(() =>
            {
                sequenceAnim = null;
            });
        }
        
        private Sequence CreateSequenceAnim(TextMeshProUGUI destinationLabel, long counterStartValue, long counterEndValue)
        {
            Sequence counterAnim = DOTween.Sequence();
            counterAnim.timeScale = Time.timeScale;
            counterAnim.Append(destinationLabel.DOScale(_scaleValue, _buttonUpDuration));
            counterAnim.Append(DOTween.To(x => destinationLabel.text = SetScoreExt.ConvertIntToStringValue((int)x, 1).ToString(), counterStartValue, counterEndValue, _textAnimDuration));
            counterAnim.Append(destinationLabel.DOScale(1, _buttonDownDuration));

            return counterAnim;
        }
        
        private void _UpdateData()
        {
            if (Stack.Count > 0)
                return;

            _UpdateCoin(_dataCentralService.StatsDataModel.CoinsCount.Value);
            _UpdateGem(_dataCentralService.StatsDataModel.GemsCount.Value);
        }
        
        private void _UpdateGem(long value)
        {
            _gemLabel.text = SetScoreExt.ConvertIntToStringValue(value, 1);
        }

        private void _UpdateCoin(long value)
        {
            _coinLabel.text = SetScoreExt.ConvertIntToStringValue(value, 1);
        }

        protected override void ResetAnimateInfluence()
        {
            _coinLabel.transform.localScale = Vector3.one;
            _gemLabel.transform.localScale = Vector3.one;
            
            _UpdateCoin(_dataCentralService.StatsDataModel.CoinsCount.Value);
            _UpdateGem(_dataCentralService.StatsDataModel.GemsCount.Value);
        }

        protected override void PopProfile()
        {
            var finance = new FinanceStack
            {
                Coin = _dataCentralService.StatsDataModel.CoinsCount.Value,
                Gem = _dataCentralService.StatsDataModel.GemsCount.Value
            };

            if (Stack.Count > 0)
            {
                var prev = (FinanceStack)Stack.Peek();
                if (prev.Coin <= finance.Coin)
                    prev.CoinNew = finance.Coin;
                if (prev.Gem <= finance.Gem)
                    prev.GemNew = finance.Gem;
            }

            Stack.Enqueue(finance);
        }
    }
}