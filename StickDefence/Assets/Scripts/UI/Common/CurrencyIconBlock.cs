using System;
using Enums;
using UI.UIManager;
using UnityEngine;

namespace UI.Common
{
    public class CurrencyIconBlock : UIBehaviour
    {
        [SerializeField] private GameObject _goldIcon;
        [SerializeField] private GameObject _gemIcon;
        [SerializeField] private GameObject _silverIcon;

        public void SetPerType(CurrencyTypeEnum currencyTypeEnum)
        {
            switch (currencyTypeEnum)
            {
                case CurrencyTypeEnum.Gold:
                    _goldIcon.SetActive(true);
                    _gemIcon.SetActive(false);
                    _silverIcon.SetActive(false);
                    break;
                case CurrencyTypeEnum.Gem:
                    _goldIcon.SetActive(false);
                    _gemIcon.SetActive(true);
                    _silverIcon.SetActive(false);
                    break;
                case CurrencyTypeEnum.Silver:
                    _goldIcon.SetActive(false);
                    _gemIcon.SetActive(false);
                    _silverIcon.SetActive(true);
                    break;
            }
        }
    }
}