using System;
using Enums;
using I2.Loc;
using UI.Common;
using UnityEngine;

namespace UI.Content.Settings
{
    public class ButtonLanguage: UIButton
    {
        [SerializeField] private GameObject _selectionMark;
        [SerializeField] private LanguageEnums _languageID;

        private bool IsSelected
        {
            set => _selectionMark.gameObject.SetActive(value);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LocalizationManager.OnLocalizeEvent += OnLocalizeEvent;
            OnClick.AddListener(OnSetLanguageClick);
            IsSelected = LocalizationManager.CurrentLanguage.Equals(_languageID.ToString());
        }

        protected override void OnDisable()
        {
            OnClick.RemoveListener(OnSetLanguageClick);
            LocalizationManager.OnLocalizeEvent -= OnLocalizeEvent;
            base.OnDisable();
        }

        private void OnLocalizeEvent()
        {
            IsSelected = LocalizationManager.CurrentLanguage.Equals(_languageID.ToString());
        }

        private void OnSetLanguageClick(UIButton uib)
        {
            LocalizationManager.CurrentLanguage = _languageID.ToString();
            
            ResourceManager.pInstance.CleanResourceCache();
            LocalizationManager.UpdateSources();
            GC.Collect();
        }
    }
}