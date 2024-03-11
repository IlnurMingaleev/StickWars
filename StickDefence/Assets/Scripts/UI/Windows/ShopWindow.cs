using System.Collections.Generic;
using UI.UIManager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class ShopWindow : Window
    {
        [SerializeField] private List<RectTransform> _rebuildRect;

        protected override void Start()
        {
            base.Start();
            foreach (var rebuildRect in _rebuildRect)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rebuildRect);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
            _manager.AddCurrentWindow(this);
        }
    }
}