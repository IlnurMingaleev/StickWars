using Cysharp.Threading.Tasks;
using Models.DataModels.Data;
using Models.DataModels.Models;
using Models.DataModels.PlatformModels;
using UniRx;
using UnityEngine;

namespace Models.DataModels
{
    public interface IDataCentralService
    {
        ISubDataModel SubData { get; }
        IStatsDataModel StatsDataModel { get; }
        IPumpingDataModel PumpingDataModel { get; }
        IMapStageDataModel MapStageDataModel { get; }
        void SaveFull();
        void Sync();
    }
    
    public class DataCentralService: IDataCentralService
    {
        private SubDataModel _subDataModel = new SubDataModel();
        private StatsDataModel _statsDataModel = new StatsDataModel();
        private PumpingDataModel _pumpingDataModel = new PumpingDataModel();
        private MapStageDataModel _mapStageDataModel = new MapStageDataModel();
        
        public ISubDataModel SubData => _subDataModel;
        public IStatsDataModel StatsDataModel => _statsDataModel;
        public IPumpingDataModel PumpingDataModel => _pumpingDataModel;
        public IMapStageDataModel MapStageDataModel => _mapStageDataModel;
        private AbstractDataPlatform _dataPlatform;

        #region PublicMethods
        
        public DataCentralService()
        {
#if UNITY_EDITOR
            _dataPlatform = new MobileDataPlatform();
#elif UNITY_WEBGL
                _dataPlatform = new WebDataPlatform();
#elif UNITY_ANDROID || UNITY_IPHONE
            _dataPlatform = new MobileDataPlatform();
#endif
            _dataPlatform.Init(_subDataModel, _statsDataModel, _pumpingDataModel, _mapStageDataModel);
            
            Observable.Timer (System.TimeSpan.FromMinutes(1)) 
                .Repeat ()
                .Subscribe (_ =>
                {
                    SaveStatsDataModel();
                }); 
        }

        public void SaveSubData()
        {
            _dataPlatform.SaveSubData();
        }
        
        public void SaveStatsDataModel()
        {
            _dataPlatform.SaveStatsDataModel();
        }
        
        public void SavePumpingDataModel()
        {
            _dataPlatform.SaveCharacterPumpingDataModel();
        }
        public void SaveMapStageDataModel()
        {
            _dataPlatform.SaveMapStageDataModel();
        }

        public void SaveFull()
        {
            SaveSubData();
            SaveStatsDataModel();
            SavePumpingDataModel();
            SaveMapStageDataModel();
        }

        public void Restart()
        {
          _subDataModel.SetAndInitEmptySubData(new SubData());
          _statsDataModel.SetAndInitEmptyStatsData(new StatsData());
          _pumpingDataModel.SetAndInitEmptyPlayerPumpingData(new PlayerPumpingData());
          _mapStageDataModel.SetAndInitEmptyMapStageData(new MapStagesData());
          SaveFull();
          _dataPlatform.Sync();
        }
        public void Sync()
        {
            _dataPlatform.Sync();
        }
        
        public async UniTask LoadData()
        {
            _dataPlatform.GetData();
            await UniTask.WaitUntil(() => _dataPlatform.IsSubDataModelLoaded
                                          && _dataPlatform.IsStatsDataModelLoaded
                                          && _dataPlatform.IsPumpingDataModelLoaded
                                          && _dataPlatform.IsMapStageDataModelLoaded);
        }
        #endregion
    }
}