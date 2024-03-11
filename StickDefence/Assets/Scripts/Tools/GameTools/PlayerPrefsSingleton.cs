using System;
using Models.DataModels.Data;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.GameTools
{
    public class PlayerPrefsSingleton : Singleton<PlayerPrefsSingleton>
    { 
        public bool IsAnyDataLoaded { private set; get; } = false;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private const string PRENAME= "FlyingTowerLands.PlayerPrefs.Dev.";
#else
        private const string PRENAME= "FlyingTowerLands.PlayerPrefs.Prod.";
#endif
        public static PlayerPrefsSingleton Instance { get; } = new PlayerPrefsSingleton();
        
        public ReactiveProperty<StatsData> StatsDataCash = new ReactiveProperty<StatsData>(new StatsData());
        public ReactiveProperty<SubData> SubDataCash = new ReactiveProperty<SubData>(new SubData());
        public ReactiveProperty<MapStagesData> MapStagesDataCash = new ReactiveProperty<MapStagesData>(new MapStagesData());
        public ReactiveProperty<PlayerPumpingData> PlayerPumpingDataCash = new ReactiveProperty<PlayerPumpingData>(new PlayerPumpingData());
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private PlayerPrefsSingleton()
        {
            StatsDataCash.Value = Load(nameof(StatsDataCash), StatsDataCash.Value);
            SubDataCash.Value = Load(nameof(SubDataCash), SubDataCash.Value);
            MapStagesDataCash.Value = Load(nameof(MapStagesDataCash), MapStagesDataCash.Value);
            PlayerPumpingDataCash.Value = Load(nameof(PlayerPumpingDataCash), PlayerPumpingDataCash.Value);
            
            StatsDataCash.SkipLatestValueOnSubscribe().Subscribe(b => Save(nameof(StatsDataCash), StatsDataCash.Value)).AddTo(_compositeDisposable);
            SubDataCash.SkipLatestValueOnSubscribe().Subscribe(b => Save(nameof(SubDataCash), SubDataCash.Value)).AddTo(_compositeDisposable);
            PlayerPumpingDataCash.SkipLatestValueOnSubscribe().Subscribe(b => Save(nameof(PlayerPumpingDataCash), PlayerPumpingDataCash.Value)).AddTo(_compositeDisposable);
            MapStagesDataCash.SkipLatestValueOnSubscribe().Subscribe(b => Save(nameof(MapStagesDataCash), MapStagesDataCash.Value)).AddTo(_compositeDisposable);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debugger.LogBold("PlayerUnit Prefs loaded");
#endif
        }

        public void Reset()
        {
            StatsDataCash.Value = new StatsData();
            SubDataCash.Value = new SubData();
            MapStagesDataCash.Value = new MapStagesData();
            PlayerPumpingDataCash.Value = new PlayerPumpingData();

            Delete(nameof(StatsDataCash));
            Delete(nameof(SubDataCash));
            Delete(nameof(MapStagesDataCash));
            Delete(nameof(PlayerPumpingDataCash));
            
            PlayerPrefs.Save();
        }
        private T Load<T>(string valueName, T defaultValue = default)
        {
            var fullPath = PRENAME + valueName;

            var str = PlayerPrefs.GetString(fullPath, default);
            if (!string.IsNullOrEmpty(str))
            {
                IsAnyDataLoaded = true;
                try
                {
                    return JsonConvert.DeserializeObject<T>(str);

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return defaultValue;
        }
        

        private void Save(string valueName, object value)
        {
            try
            {
                PlayerPrefs.SetString(PRENAME + valueName,  JsonConvert.SerializeObject(value));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            PlayerPrefs.Save();
        }
        
        private void Delete(string valueName)
        {
            try
            {
                PlayerPrefs.DeleteKey(PRENAME + valueName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}