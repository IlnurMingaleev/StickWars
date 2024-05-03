using Models.DataModels.Data;
using Models.DataModels.Models;
using Models.Merge;
using Tools.GameTools;

namespace Models.DataModels.PlatformModels
{
    public class MobileDataPlatform : AbstractDataPlatform
    {
        private JsonSerialization<SubData> _jsonSerializationSubData = new((PRENAME + nameof(SubDataModel) + ".json"));
        private JsonSerialization<StatsData> _jsonSerializationStatsData = new((PRENAME + nameof(StatsDataModel) + ".json"));
        private JsonSerialization<PlayerPumpingData> _jsonSerializationCharacterPumpingData = new((PRENAME + nameof(PumpingDataModel) + ".json"));
        private JsonSerialization<MapStagesData> _jsonSerializationCharacterMapStageData = new((PRENAME + nameof(MapStageDataModel) + ".json"));
        #region Save
        public override void SaveSubData() => _jsonSerializationSubData.Serialization(SubDataModel.GetSubData());
        public override void SaveStatsDataModel() => _jsonSerializationStatsData.Serialization(StatsDataModel.GetStatsData());
        public override void SaveCharacterPumpingDataModel() => _jsonSerializationCharacterPumpingData.Serialization(PumpingDataModel.GetData());
        public override void SaveMapStageDataModel() => _jsonSerializationCharacterMapStageData.Serialization(MapStageDataModel.GetMapStageData());

        #endregion
        
        #region Load
        public override void GetData()
        {
            LoadSubDataModel(_jsonSerializationSubData.DeSerialization());
            LoadStatsDataModel(_jsonSerializationStatsData.DeSerialization());
            LoadCharacterPumpingDataModel(_jsonSerializationCharacterPumpingData.DeSerialization());
            LoadMapStageDataModel(_jsonSerializationCharacterMapStageData.DeSerialization());
        }
        private void LoadSubDataModel((bool result, SubData subData) result)
        {
            if (result.result)
                SubDataModel.SetSubData(result.subData);
            else
                SubDataModel.SetAndInitEmptySubData(result.subData);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debugger.LogBold((PRENAME + nameof(SubData)) + "EndLoad");
#endif
            
            IsSubDataModelLoaded = true;
        }
        
        private void LoadStatsDataModel((bool result, StatsData statsData) result)
        {
            if (result.result)
                StatsDataModel.SetStatsData(result.statsData);
            else
                StatsDataModel.SetAndInitEmptyStatsData(new StatsData());

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debugger.LogBold((PRENAME + nameof(StatsDataModel)) + "EndLoad");
#endif
            
            IsStatsDataModelLoaded = true;
        }
        

        private void LoadCharacterPumpingDataModel((bool result, PlayerPumpingData characterPumpingData) result)
        {
            if (result.result)
                PumpingDataModel.SetPlayerPumpingData(result.characterPumpingData);
            else
                PumpingDataModel.SetAndInitEmptyPlayerPumpingData(new PlayerPumpingData());

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debugger.LogBold((PRENAME + nameof(PumpingDataModel)) + "EndLoad");
#endif
            
            IsPumpingDataModelLoaded = true;
        }
        
        private void LoadMapStageDataModel((bool result, MapStagesData mapStageData) result)
        {
            if (result.result)
                MapStageDataModel.SetMapStageData(result.mapStageData);
            else
                MapStageDataModel.SetAndInitEmptyMapStageData(new MapStagesData());

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debugger.LogBold((PRENAME + nameof(MapStageDataModel)) + "EndLoad");
#endif
            
            IsMapStageDataModelLoaded = true;
        }

    #endregion
    }
}