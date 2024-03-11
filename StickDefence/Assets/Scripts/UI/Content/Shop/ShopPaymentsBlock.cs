using Enums;
using Models.Controllers;
using Models.DataModels;
using Models.IAP;
using Models.Player;
using UI.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;


namespace UI.Content.Shop
{
    public class ShopPaymentsBlock : UIBehaviour
    {
        [SerializeField] private GameObject _gemPack;
        [SerializeField] private GameObject _adsRemovePack;
        [SerializeField] private RectTransform _rebuildRect;
        
        [Inject] private readonly IIAPService _iapService;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly IPlayer _player;
        
        protected override void Awake()
        {
            base.Awake();
            _gemPack.SetActive(_iapService.PaymentModel.IsPaymentsAvailable);
            _adsRemovePack.SetActive(_iapService.PaymentModel.IsPaymentsAvailable);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (_iapService.PaymentModel.IsPaymentsAvailable)
            {
                _dataCentralService.SubData.IsADSRemove.TakeUntilDisable(this).Subscribe(value =>
                {
                    _adsRemovePack.gameObject.SetActive(!value);
                });
            }
        }
    }
}