using GamePush;
using Models.DataModels.Data;
using Newtonsoft.Json;
using Tools.GameTools;
using UnityEngine;

namespace Models.DataModels.PlatformModels
{
    public class EditorDataPlatform : AbstractDataPlatform
    {
        #region Save
        public override void SaveSubData()
        {
            PlayerPrefs.SetString((PRENAME + nameof(SubDataModel)), JsonConvert.SerializeObject((object)SubDataModel.GetSubData()));
            PlayerPrefs.Save();
        }

        public override void SaveStatsDataModel()
        {
            PlayerPrefs.SetString((PRENAME + nameof(StatsDataModel)), JsonConvert.SerializeObject((object)StatsDataModel.GetStatsData()));
            PlayerPrefs.Save();
        }

        #endregion
        public override void GetData()
        {
            LoadSubDataModel(PlayerPrefs.GetString(PRENAME + nameof(SubDataModel)));
            LoadStatsDataModel(PlayerPrefs.GetString(PRENAME + nameof(StatsDataModel)));
        }
        
        #region Load
        private void LoadSubDataModel(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debugger.LogBold((PRENAME + nameof(SubData)) + "EndLoad");
#endif
                SubDataModel.SetSubData(JsonConvert.DeserializeObject<SubData>(result));
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debugger.LogBold((PRENAME + nameof(SubData)) + "Create new");
#endif
                SubDataModel.SetAndInitEmptySubData(new SubData());
            }

            IsSubDataModelLoaded = true;
        }

        private void LoadStatsDataModel(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debugger.LogBold((PRENAME + nameof(StatsDataModel)) + "EndLoad");
#endif
                StatsDataModel.SetStatsData(JsonConvert.DeserializeObject<StatsData>(result));
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debugger.LogBold((PRENAME + nameof(StatsDataModel)) + "Create new");
#endif
                StatsDataModel.SetAndInitEmptyStatsData(new StatsData());
            }

            IsStatsDataModelLoaded = true;
        }

    #endregion
    }
}