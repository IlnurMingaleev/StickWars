using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Views.Home
{
    public class LobbyGameButton : MonoBehaviour
    {
        [SerializeField] private DOTweenAnimation _buttonTween;
        [SerializeField] private Collider2D _colliderButton;

        private void OnEnable()
        {
            _buttonTween.onComplete.AddListener(OnAnimationEnd);
        }

        private void OnDisable()
        {
            _buttonTween.onComplete.RemoveAllListeners();
            _colliderButton.enabled = true;
        }

        public void AddListener(UnityAction listener)
        {
            _buttonTween.onComplete.AddListener(listener);
        }
        private void OnMouseDown()
        {
            _colliderButton.enabled = false;
            _buttonTween.DORestart();
            _buttonTween.DOPlay();
        }

        private void OnAnimationEnd()
        {
            _colliderButton.enabled = true;
        }
    }
}