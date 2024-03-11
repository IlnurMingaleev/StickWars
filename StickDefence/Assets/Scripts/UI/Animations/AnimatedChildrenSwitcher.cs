using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Animations
{
    public class AnimatedChildrenSwitcher : UIBehaviour
    {
        [SerializeField] private float _showingTime;
        [SerializeField] private float _switchHalfTime;
        [SerializeField] private Ease _compressionEase;
        [SerializeField] private Ease _expansionEase;

        private Sequence _sequence;

        protected override void Awake()
        {
            base.Awake();

            _sequence = DOTween.Sequence().SetLoops(-1);

            foreach (Transform child in transform)
            {
                _sequence.AppendCallback(() => child.gameObject.SetActive(true));
                _sequence.Append(child.DOScaleX(1, _switchHalfTime)
                    .ChangeStartValue(new Vector3(0, 1, 1))
                    .SetEase(_expansionEase));
                _sequence.AppendInterval(_showingTime);
                _sequence.Append(child.DOScaleX(0, _switchHalfTime)
                    .ChangeStartValue(Vector3.one)
                    .SetEase(_compressionEase));
                _sequence.AppendCallback(() => child.gameObject.SetActive(false));
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
            _sequence.Restart();
        }

        protected override void OnDisable()
        {
            _sequence.Pause();
            base.OnDisable();
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            _showingTime = 1f;
            _switchHalfTime = 1f;

            _compressionEase = Ease.InCubic;
            _expansionEase = Ease.OutCubic;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (transform.childCount == 0)
                UnityEngine.Debug.LogWarning($"{nameof(AnimatedChildrenSwitcher)}.{nameof(OnValidate)}::Unused component. Transform hasn't children.");
        }
#endif
    }
}
