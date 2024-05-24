using System;
using System.Collections;
using System.Collections.Generic;
using Models.DataModels;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;

namespace TonkoGames.Sound
{
    public interface ISoundManager
    {
        void PlayLooseSourceOneShot();
        void PlayBoosterSourceOneShot();
        void PlayUiButtonClick();
        void PlayMusic();
        void StopPlayMusic();
        void PlayExplosionOneShot();
        void PlayWinSourceOneShot();
        void PlayBulletFireOneShot();
        void PlayEnemyDeadOneShot();
        void PlayPickUpOneShot();
        public void PlayPutOneShot();
        void PlayMoneySoundOneShot();

    }
    public class SoundManager : MonoBehaviour, ISoundManager
    {
       [Header("Source")] [SerializeField] private AudioSource _soundSource;
        [SerializeField] private SourceSound _soundSourceSound;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private SourceSound _musicSourceSound;
        [Header("Streaming Assets")]
        [SerializeField] private List<AudioClip> _audioClips;
        [SerializeField] private List<string> _clipUrl;
        private bool _isAssetsLoaded = false;
        private bool _inProcess = false;
        [Inject] private readonly IDataCentralService _dataCentralService;


        private void Awake()
        {
            Init();
        }
        

        private void OnDisable()
        {
            StopPlayMusic();
        }

        #region Value

        private void OnEnable()
        {
            _soundSourceSound.Init(_dataCentralService.SubData.SoundVolume);
            _musicSourceSound.Init(_dataCentralService.SubData.MusicVolume);
        }

        #endregion

        #region Music
        private void Start()
        {
            _isAssetsLoaded = false;
            _inProcess = false;
            PlayMusic();
        }

        public void Init()
        {
            if (_inProcess) return;
            if (_isAssetsLoaded) return;
            _inProcess = true;
            LoadAssets();
        }

        public void PlayOneShot(int id)
        {
            if (id >= _audioClips.Count) return;
            if (_audioClips[id] == null) return;
            _soundSource.PlayOneShot(_audioClips[id]);
        }

        public void PlayMusic()
        {
            if ((int)MusicTypeEnum.MUSIC >= _audioClips.Count) return;
            if (_audioClips[(int)MusicTypeEnum.MUSIC] == null) return;
            _musicSource.clip = _audioClips[(int)MusicTypeEnum.MUSIC];
            _musicSource.Play();
        }

        public void StopPlayMusic()
        {
            _musicSource.Stop();
        }

      
        #endregion

        #region Ui
        

        public void PlayUiButtonClick() => PlayOneShot((int)MusicTypeEnum.UICLICK);
        public void PlayPickUpOneShot() => PlayOneShot((int)MusicTypeEnum.PICKUP);
        public void PlayPutOneShot() => PlayOneShot((int)MusicTypeEnum.PUT);

        #endregion

        #region Money
        

        public void PlayMoneySoundOneShot() => PlayOneShot((int)MusicTypeEnum.BUYACTION);

        #endregion
        #region Explosion
        public void PlayExplosionOneShot() => PlayOneShot((int) MusicTypeEnum.EXPLOSION);
        
        public void PlayBulletFireOneShot() => PlayOneShot((int) MusicTypeEnum.BULLETFIRE);
        #endregion

        #region Loose

        public void PlayLooseSourceOneShot() => PlayOneShot((int)MusicTypeEnum.LOSTGAME);

        #endregion

        #region Checkpoint

        public void PlayCheckpointSourceOneShot() => PlayOneShot((int)MusicTypeEnum.BUYACTION);

        #endregion


        #region Booster

        public void PlayBoosterSourceOneShot() => PlayOneShot((int)MusicTypeEnum.BOOSTERCOLLECT);

        #endregion

        #region Win

        public void PlayWinSourceOneShot() => PlayOneShot((int)MusicTypeEnum.WONGAME);

        #endregion
        #region Enemy

        public void PlayEnemyDeadOneShot() => PlayOneShot((int)MusicTypeEnum.ENEMYDEATH);

        #endregion
        

        #region -LOAD ASSETS-

        

        private void LoadAssets()
        {
            var url = Application.streamingAssetsPath + "/";
            foreach (var clip in _clipUrl)
            {
                _audioClips.Add(null);
                StartCoroutine(LoadClip(url + clip + ".mp3", _audioClips.Count - 1));
            }
        }

        IEnumerator LoadClip(string url, int id)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    _audioClips[id] = DownloadHandlerAudioClip.GetContent(www);
                    if (IsDone())
                    {
                        _inProcess = false;
                        _isAssetsLoaded = true;
                    }
                }
                else
                {
                    Debug.Log(www.error);
                }
            }
        }

        private bool IsDone()
        {
            for (int i = 0; i < _audioClips.Count; i++)
            {
                if (_audioClips[i] == null) return false;
            }

            return true;
        }

        #endregion

    }
    public enum MusicTypeEnum
    {
        MUSIC = 0,
        UICLICK = 1,
        BUYACTION = 2,
        BOOSTERCOLLECT = 3,
        LOSTGAME = 4,
        WONGAME = 5,
        BULLETFIRE = 6,
        EXPLOSION = 7,
        ENEMYDEATH = 8,
        PICKUP = 9,
        PUT = 10,

    }
}