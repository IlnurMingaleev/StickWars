using DG.Tweening;

namespace UI.Animations
{
    public class WindowAnimation : DOTweenAnimation
    {
        private DOTweenVisualManager _tweenVisualManager;
        private bool _requiresRestartFromSpawnPoint;
        private bool _isGetVisualManager;

        private void Update()
        {
            if (!this._requiresRestartFromSpawnPoint)
                return;
            this._requiresRestartFromSpawnPoint = false;
            this.DORestart(true);
        }

        private void OnEnable()
        {
            isIndependentUpdate = true;
            if (GetVisualManagerOnEnableBehaviour() == DG.Tweening.Core.OnEnableBehaviour.RestartFromSpawnPoint)
                _requiresRestartFromSpawnPoint = true;
            else
                this.DORestart(false);
        }

        private DG.Tweening.Core.OnEnableBehaviour GetVisualManagerOnEnableBehaviour()
        {
            if (!_isGetVisualManager)
            {
                _tweenVisualManager = GetComponent<DOTweenVisualManager>();
            }
            _isGetVisualManager = true;

            return _tweenVisualManager ? _tweenVisualManager.onEnableBehaviour : DG.Tweening.Core.OnEnableBehaviour.None;
        }

        public void ChangeValue(object endValue)
        {
            ((Tweener)tween).ChangeEndValue(endValue);
        }
    }
}