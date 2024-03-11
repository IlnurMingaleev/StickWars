using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Views.Health
{
    public class DamageableFlashAnim : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private TweenerCore<float, float, FloatOptions> _tweenFlash;
        private readonly int _flashAmount = Shader.PropertyToID("_FlashAmount");

#if UNITY_EDITOR
        private void OnValidate()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
#endif
        
        public void Flash()
        {
            if (_tweenFlash != null)
                return;

            _tweenFlash = _spriteRenderer.material.DOFloat(1, "_FlashAmount", 0.25f).OnComplete(() => {  
                _tweenFlash = _spriteRenderer.material.DOFloat(0, "_FlashAmount", 0.25f).OnComplete(() =>
                {
                    _tweenFlash = null;
                });
            });

        }

        public void StopFlash()
        {
            if (_tweenFlash == null) return;
            
            _tweenFlash.Pause();
            _spriteRenderer.material.SetFloat(_flashAmount, 0);
            _tweenFlash = null;
        }
    }
}