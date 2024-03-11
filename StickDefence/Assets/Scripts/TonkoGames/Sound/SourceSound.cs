using Models.DataModels.Models;
using UniRx;
using UnityEngine;

namespace TonkoGames.Sound
{
    public class SourceSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _maxVolumeSource;

        private ISubDataModel _subDataModel;

        private CompositeDisposable _disposable = new CompositeDisposable();

        private IReadOnlyReactiveProperty<float> _volume;

        private void OnEnable()
        {
            SubVolume();
        }

        public void Init(IReadOnlyReactiveProperty<float> volume)
        {
            _volume = volume;
            SubVolume();
        }

        private void SubVolume()
        {
            if (_volume != null)
            {
                _disposable.Clear();
                _volume.Subscribe(VolumeChange).AddTo(_disposable);
            }
        }

        private void VolumeChange(float value) => _audioSource.volume = _maxVolumeSource * value;

        public void UpdateMaxVolumeSource(float value)
        {
            _maxVolumeSource = value;
            VolumeChange(_volume.Value);
        }
    }
}