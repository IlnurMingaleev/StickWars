using System;
using System.Collections.Generic;
using System.Linq;
using Tools.Extensions;
using UI.Animations;
using UniRx;
using UnityEngine;

namespace UI.UIManager
{
 public abstract class Window : UIBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private bool _isUndestroyable;
        [SerializeField] private bool _isTransparent = true;
        [SerializeField] private List<WindowAnimation> _windowAnimations;
#pragma warning restore 0649
        public CanvasGroup CanvasGroup => this.GetComponent<CanvasGroup>();
        
        public IEnumerable<WindowAnimation> WindowAnimations
        {
            get
            {
#if UNITY_EDITOR
                if (_windowAnimations.IsEmpty())
                {
                    GetComponentsInChildren(_windowAnimations);

                    if (!_windowAnimations.IsEmpty())
                    {
                        UnityEngine.Debug.LogWarning($"WindowAnimations in {name}.prefab is NULL. Need resave prefab.",
                            UnityEditor.PrefabUtility.GetNearestPrefabInstanceRoot(this));

                        _windowAnimations.Clear();
                    }
                }
#endif
                return _windowAnimations.Where(a => a.gameObject.activeInHierarchy);
            }
        }
        
        [NonSerialized]
        protected WindowPriority _windowPriority = WindowPriority.Deffault;
        
        public virtual string LocalizedName => "-"; // "-" in I2Localization is empty term
        
        public virtual WindowPriority Priority
        {
            get => _windowPriority;
            set => _windowPriority = value;
        }

        public bool IsShowing => this.gameObject.activeSelf;
        [NonSerialized]
        public ReactiveProperty<bool> IsShowingReactive = new ReactiveProperty<bool>(false); 
        
        public string Name => name;
        public bool IsUndestroyable => _isUndestroyable;
        public bool IsTransparent => _isTransparent;
        
        public Transform Parent {
            set => this.transform.SetParent(value, false);
        }
        
        protected IWindowManager _manager;
        
        protected virtual bool DisableMultiTouchOnShow => true;
        private IDisposable _multiTouchDisposable;
        protected CompositeDisposable ActivateDisposables { get; private set; }
        
        public void Setup(IWindowManager manager)
        {
            _manager = manager;
            _windowAnimations.ForEach(windowAnimation => windowAnimation.isIndependentUpdate = true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            OnDeactivate();

            // if (DisableMultiTouchOnShow)
            //     _multiTouchDisposable?.Dispose();
        }

        public void Show()
        {
            if (DisableMultiTouchOnShow)
            {
                // _multiTouchDisposable?.Dispose();
                // _multiTouchDisposable = GlobalMultiTouchCounter.GetDisableToken();
            }

            this.gameObject.SetActive(true);
            OnActivate();
        }
                
        public int GetSiblingIndex()
        {
            return this.transform.GetSiblingIndex();
        }

        public void SetSiblingIndex(int index)
        {
            this.transform.SetSiblingIndex(index);
        }

        public void SetAsLastSibling()
        {
            this.transform.SetAsLastSibling();
        }

        public void SetAsFirstSibling()
        {
            this.transform.SetAsFirstSibling();
        }
        protected sealed override void OnEnable()
        {
            IsShowingReactive.Value = true;
        }

        protected sealed override void OnDisable()
        {
            IsShowingReactive.Value = false;
        }
        protected virtual void OnActivate()
        {
            if (ActivateDisposables == null)
                ActivateDisposables = new CompositeDisposable().AddTo(this);
            else
                ActivateDisposables.Clear();
        }

        protected virtual void OnDeactivate()
        {
            ActivateDisposables?.Clear();
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (Application.isPlaying)
                return;

            var anims = _windowAnimations ?? (_windowAnimations = new List<WindowAnimation>());
            int oldCount = anims.Count;

            GetComponentsInChildren(anims);
            if (anims.Count - oldCount > 0)
                UnityEngine.Debug.LogFormat($"OnValidate: {name} Added: {anims.Count - oldCount}");
        }
#endif
        
    }
}