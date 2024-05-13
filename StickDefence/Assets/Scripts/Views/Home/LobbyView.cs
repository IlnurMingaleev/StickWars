using System;
using System.Collections.Generic;
using System.Linq;
using Tools.GameTools;
using UI.Content.Lobby;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;

namespace Views.Home
{
    public class LobbyView : MonoBehaviour
    {
       
        [SerializeField] private GameObject _blocker;
        //[SerializeField] private ScaleCameraSize2D _cameraScaler;
        
        [Inject] private readonly IWindowManager _windowManager;

        private CompositeDisposable _disposables = new CompositeDisposable();

        private void OnEnable()
        {
          
            _windowManager.GetWindow<LobbyWindow>().CanClickableLobby.Subscribe(value => _blocker.SetActive(!value)).AddTo(_disposables);
            
            _blocker.SetActive(false);
            //_cameraScaler.StartSnap(SnapCamera.Bottom);
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }
        
        private void OnShopButton()
        {
            _windowManager.Show<ShopWindow>();
            _windowManager.Hide<LobbyWindow>();
        }
        
        private void OnStorageButton()
        {
            _windowManager.GetWindow<LobbyWindow>().OpenStorage();
        }
        private void OnCoinSmeltingButton()
        {
            _windowManager.GetWindow<LobbyWindow>().OpenCoinSmelting();
        }
        private void OnUpgradeBaseButton()
        {
            _windowManager.GetWindow<LobbyWindow>().OpenUpgradeBase();
        }
    }
}