using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.DataModels;
using Models.IAP;
using TMPro;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Windows
{
    public class SettingsWindow : Window
    {
        [SerializeField] private UIButton _closeWindow;
        [SerializeField] private Slider _soundSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private TMP_Text _uidLabel;
        [SerializeField] private UIButton _resetProgressBtn;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly IIAPService _iapService;
        
        protected override void Awake()
        {
            base.Awake();
            string uid = _iapService.GetUID();

            if (string.IsNullOrEmpty(uid))
            {
                _uidLabel.gameObject.SetActive(false);
            }
            else
            {
                _uidLabel.text = "UID: " + uid;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _resetProgressBtn.OnClickAsObservable.Subscribe(_ => _dataCentralService.Restart());
            _soundSlider.value = _dataCentralService.SubData.SoundVolume.Value;
            _musicSlider.value = _dataCentralService.SubData.MusicVolume.Value;
            
            _soundSlider.onValueChanged.AddListener(SoundSliderChange);
            _musicSlider.onValueChanged.AddListener(MusicSliderChange);
            _closeWindow.OnClickAsObservable.Subscribe(_ => _manager.Hide(this)).AddTo(ActivateDisposables);
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Pause);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(_coreStateMachine.RunTimeStateMachine.LastRunTimeState);
            _dataCentralService.SaveFull();
        }


        private void SoundSliderChange(float value)
        {
            _dataCentralService.SubData.SetSoundVolume(value);
        }
        
        private void MusicSliderChange(float value)
        {
            _dataCentralService.SubData.SetMusicVolume(value);  
        }
    }
}