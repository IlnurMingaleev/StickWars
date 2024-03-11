using GamePush;
using Models.DataModels.Data;
using Newtonsoft.Json;
using Tools.GameTools;

namespace Models.DataModels.PlatformModels
{
#if UNITY_WEBGL || UNITY_EDITOR
    public class WebDataPlatform : AbstractDataPlatform
    {
        private bool _loadFromLocal = false;
        public override void Sync()
        {           
            GamePush.GP_Player.Sync();
        }
        
        #region Save
        public override void SaveSubData()
        {
            GP_Player.Set((PRENAME + nameof(SubDataModel)), JsonConvert.SerializeObject((object)SubDataModel.GetSubData()));
        }
        
        public override void SaveStatsDataModel()
        {
            GP_Player.Set((PRENAME + nameof(StatsDataModel)), JsonConvert.SerializeObject((object)StatsDataModel.GetStatsData()));
        }
        
        public override void SaveCharacterPumpingDataModel()
        {
            GP_Player.Set((PRENAME + nameof(PumpingDataModel)), JsonConvert.SerializeObject((object)PumpingDataModel.GetCharacterPumpingData()));
        }
        
        public override void SaveMapStageDataModel()
        {
            GP_Player.Set((PRENAME + nameof(MapStageDataModel)), JsonConvert.SerializeObject((object)MapStageDataModel.GetMapStageData()));
        }

        #endregion
        public override void GetData()
        {
            LoadStatsDataModel(GP_Player.GetString(PRENAME + nameof(StatsDataModel)));
            LoadSubDataModel(GP_Player.GetString(PRENAME + nameof(SubDataModel)));
            LoadPumpingDataModel(GP_Player.GetString(PRENAME + nameof(PumpingDataModel)));
            LoadMapStageDataModel(GP_Player.GetString(PRENAME + nameof(MapStageDataModel)));
        }
        
        #region Load
        private void LoadStatsDataModel(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                var statsDataLoad = JsonConvert.DeserializeObject<StatsData>(result);
                if (PlayerPrefsSingleton.Instance.StatsDataCash.Value.LastDataTimeVisit > statsDataLoad.LastDataTimeVisit)
                {
                    StatsDataModel.SetStatsData(PlayerPrefsSingleton.Instance.StatsDataCash.Value);
                    _loadFromLocal = true;
                }
                else
                {
                    StatsDataModel.SetStatsData(statsDataLoad);
                }
            }
            else
            {
                StatsDataModel.SetAndInitEmptyStatsData(new StatsData());
                SaveStatsDataModel();
            }
            
            Debugger.LogBold((PRENAME + nameof(StatsDataModel)) + "EndLoad");
            IsStatsDataModelLoaded = true;
        }
        private void LoadSubDataModel(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                if (_loadFromLocal)
                {
                    SubDataModel.SetSubData(PlayerPrefsSingleton.Instance.SubDataCash.Value);
                }
                else
                {
                    SubDataModel.SetSubData(JsonConvert.DeserializeObject<SubData>(result));
                }
            }
            else
            {
                SubDataModel.SetAndInitEmptySubData(new SubData());
                SaveSubData();
            }
            
            Debugger.LogBold((PRENAME + nameof(SubData)) + "EndLoad");
            
            IsSubDataModelLoaded = true;
        }

        
        
        private void LoadPumpingDataModel(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                if (_loadFromLocal)
                {
                    PumpingDataModel.SetPlayerPumpingData(PlayerPrefsSingleton.Instance.PlayerPumpingDataCash.Value);
                }
                else
                {
                    PumpingDataModel.SetPlayerPumpingData(JsonConvert.DeserializeObject<PlayerPumpingData>(result));
                }
            }
            else
            {
                PumpingDataModel.SetAndInitEmptyPlayerPumpingData(new PlayerPumpingData());
                SaveCharacterPumpingDataModel();
            }
            
            Debugger.LogBold((PRENAME + nameof(PumpingDataModel)) + "EndLoad");
            
            IsPumpingDataModelLoaded = true;
        }
        
        private void LoadMapStageDataModel(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                if (_loadFromLocal)
                {
                    MapStageDataModel.SetMapStageData(PlayerPrefsSingleton.Instance.MapStagesDataCash.Value);
                }
                else
                {
                    MapStageDataModel.SetMapStageData(JsonConvert.DeserializeObject<MapStagesData>(result));
                }
            }
            else
            {
                MapStageDataModel.SetAndInitEmptyMapStageData(new MapStagesData());
                SaveMapStageDataModel();
            }
            
            Debugger.LogBold((PRENAME + nameof(MapStageDataModel)) + "EndLoad");
            
            IsMapStageDataModelLoaded = true;
        }

    #endregion
    }
#endif
}