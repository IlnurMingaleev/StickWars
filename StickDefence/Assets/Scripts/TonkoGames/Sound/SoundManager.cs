using System;
using System.Collections.Generic;
using Models.DataModels;
using UnityEngine;
using VContainer;

namespace TonkoGames.Sound
{
    public interface ISoundManager
    {
        void PlayMusic();
        void StopPlayMusic();
        void PlayLoseGameMusic();
        void StopLoseGameMusic();

        void PlayExplosionSourceOneShot(AudioClip pickUpSound);
        void PlayUiSourceOneShot(AudioClip pickUpSound);
        void PlayUiButtonClick();
    }
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        [Header("Source")] 
        [SerializeField] private AudioSource _explosionSource;
        [SerializeField] private SourceSound _explosionSourceSound;
        [SerializeField] private AudioSource _audioSourceUi;
        [SerializeField] private SourceSound _uiSourceSound;

        [Header("Sub Clips")]
        [SerializeField] private AudioClip _clickUi;


        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _loseGameMusicSource;


        [Inject] private readonly IDataCentralService _dataCentralService;

        #region Value

        private void OnEnable()
        {
            _uiSourceSound.Init(_dataCentralService.SubData.SoundVolume);
            _explosionSourceSound.Init(_dataCentralService.SubData.SoundVolume);
        }

        #endregion

        #region Music

        public void PlayMusic()
        {
//            _musicSource.Play();
        }
        
        public void StopPlayMusic()
        {         
          //  _musicSource.Stop();
        }

        public void PlayLoseGameMusic()
        {
            //_loseGameMusicSource.Play();
        }
        public void StopLoseGameMusic() => _loseGameMusicSource.Stop();

        
        #endregion

        #region Ui

        public void PlayUiSourceOneShot(AudioClip audioClip) => _audioSourceUi.PlayOneShot(audioClip);

        public void PlayUiButtonClick() => PlayUiSourceOneShot(_clickUi);

        #endregion
        
        
        #region Explosion

        public void PlayExplosionSourceOneShot(AudioClip audioClip) => _explosionSource.PlayOneShot(audioClip);

        #endregion

    }
}