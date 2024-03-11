using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI.Animations
{
    public class DoTweenSequence : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private List<DOTweenAnimation> _animations;
        [SerializeField] private int _loops = 0;
#pragma warning restore 0649

        private Sequence _sequence;

        private void Awake()
        {
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);
            _sequence.SetLoops(_loops);
            foreach (var anim in _animations)
            {
                if (anim != null && anim.tween != null)
                    _sequence.Append(anim.tween);
            }
        }

        private void OnEnable() => _sequence.Restart();

#if UNITY_EDITOR
        public List<DOTweenAnimation> Animations => _animations;
#endif
    }
}