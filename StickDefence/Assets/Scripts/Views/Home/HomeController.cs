using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;

namespace Views.Home
{
    public class HomeController : MonoBehaviour
    {
        [SerializeField] private GameObject _lobby;

        [Inject] private readonly IWindowManager _windowManager;

        private void OnEnable()
        {
            _windowManager.GetWindow<LobbyWindow>().IsShowingReactive.TakeUntilDisable(this)
                .Subscribe(value => _lobby.gameObject.SetActive(value));
        }
    }
}