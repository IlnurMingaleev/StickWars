using Models.DataModels.Models;

namespace Models.DataModels.PlatformModels
{
    public class AbstractDataPlatform
    {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        protected const string PRENAME= "StickDefenceDev";
#else
        protected const string PRENAME= "StickDefenceStorage";
#endif
        public bool IsSubDataModelLoaded = false;
        public bool IsStatsDataModelLoaded = false;
        public bool IsPumpingDataModelLoaded = false;
        public bool IsMapStageDataModelLoaded = false;
        
        protected SubDataModel SubDataModel;
        protected StatsDataModel StatsDataModel;
        protected PumpingDataModel PumpingDataModel;
        protected MapStageDataModel MapStageDataModel;
        
        public void Init(SubDataModel subDataModel, StatsDataModel statsDataModel, PumpingDataModel pumpingDataModel,
            MapStageDataModel mapStageDataModel)
        {
            SubDataModel = subDataModel;
            StatsDataModel = statsDataModel;
            PumpingDataModel = pumpingDataModel;
            MapStageDataModel = mapStageDataModel;
        }
        
        public void SaveFullData()
        {
            SaveSubData();
            SaveStatsDataModel();
            SaveCharacterPumpingDataModel();
            SaveMapStageDataModel();
        }
        public virtual void SaveSubData()
        {
        }

        public virtual void SaveStatsDataModel()
        {
        }
        public virtual void SaveCharacterPumpingDataModel()
        {
        }

        public virtual void SaveMapStageDataModel()
        {
            
        }
        public virtual void GetData()
        {
        }
        public virtual void Sync()
        {
            
        }
    }
}