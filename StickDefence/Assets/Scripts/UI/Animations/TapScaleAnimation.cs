using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Animations
{
    public class TapScaleAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Transform _target;

        [Header("Animation parameters")] [SerializeField]
        private float _scale = 1.15f;

        [SerializeField] private float _increaseDuration = .05f;
        [SerializeField] private Ease _increaseEasing = Ease.OutExpo;
        [SerializeField] private float _decreaseDuration = .05f;
        [SerializeField] private Ease _decreaseEasing = Ease.Linear;

        [Header("Additionally")] [SerializeField]
        private bool _waitUntilDecrease = false;

        private Tween _increaseAnimation;
        private Tween _decreaseAnimation;

        private bool _pointerUpped;
        private bool _increaseCompleted;

        private bool _pointerUpInteractable;
        private bool _pointerDownInteractable;

        private bool _wasPointerDown;

        private Selectable _selectableTarget;

        //private GlobalWindowBlocker blocker;

        #region Хак, использовать с соторожностью

        public Transform Target
        {
            get => _target;
            set
            {
                _target = value;
                RebuildAnimation();
            }
        }

        #endregion

        public event Action AnimationEvent = () => { };
        public event Action Completed = () => { };

        private void Awake()
        {
            RebuildAnimation();
            _pointerUpInteractable = true;
            _pointerDownInteractable = true;
        }

        private void RebuildAnimation()
        {
            _selectableTarget = _target != null ? _target.GetComponent<Selectable>() : null;

            if (_target == null)
                return;

            if (_increaseAnimation != null)
            {
                _increaseAnimation.Kill();
                _increaseAnimation = null;
            }

            if (_decreaseAnimation != null)
            {
                _decreaseAnimation.Kill();
                _decreaseAnimation = null;
            }

            var startValue = _target.localScale;
            var endValue = new Vector3(_scale, _scale, _scale);

            _increaseAnimation = DOTween.To(() => _target.localScale, (newScale) => _target.localScale = newScale,
                    endValue, _increaseDuration)
                .SetEase(_increaseEasing)
                .OnComplete(() =>
                {
                    _increaseCompleted = true;
                    if (_pointerUpped)
                        Decrease();
                })
                .Pause()
                .SetAutoKill(false);

            _decreaseAnimation = DOTween.To(() => _target.localScale, (newScale) => _target.localScale = newScale,
                    startValue, _decreaseDuration)
                .SetEase(_decreaseEasing)
                .OnComplete(() =>
                {
                    _pointerUpInteractable = true;
                    _pointerDownInteractable = true;
                    if (_waitUntilDecrease)
                        AnimationEvent();
                    Completed();
                })
                .Pause()
                .SetAutoKill(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_target == null)
                return;

            if (!_pointerDownInteractable)
                return;

            if (_selectableTarget && !_selectableTarget.interactable)
                return;

            _pointerDownInteractable = false;
            _wasPointerDown = true;

            if (_increaseAnimation != null)
                _increaseAnimation.Restart();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_target == null)
                return;

            if (!_pointerUpInteractable)
                return;

            if (!_wasPointerDown)
                return;

            _pointerUpInteractable = false;
            _wasPointerDown = false;

            if (!_increaseCompleted)
            {
//            blocker?.Dispose();
//            blocker = GlobalWindowBlockerDisposable.SetBlock();
            }

            _pointerUpped = true;
            if (_increaseCompleted)
                Decrease();
        }

        private void Decrease()
        {
//        blocker?.Dispose();
//        blocker = null;

            _pointerUpped = false;
            _increaseCompleted = false;

            if (!_waitUntilDecrease)
                AnimationEvent();
            if (_decreaseAnimation != null)
                _decreaseAnimation.Restart();
        }
    }
}