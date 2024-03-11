using System.Collections.Generic;
using System.Linq;
using Enums;
using Models.DataModels.Data;
using UniRx;

namespace Models.DataModels.Models
{
    public interface IMapStageDataModel
    {
        #region Fields
        IReadOnlyReactiveDictionary<MapStagesEnum, MapStageData> MapStages { get; }

        #endregion
        
        #region Setters
        void UpdateMapStageData(MapStageData mapStageData);
        MapStageData GetMapStageData(MapStagesEnum mapStageType);

        #endregion
    } 
    public class MapStageDataModel : IMapStageDataModel
    {
        #region Fields

        private ReactiveDictionary<MapStagesEnum, MapStageData> _mapStages =
            new ReactiveDictionary<MapStagesEnum, MapStageData>();
        
        public IReadOnlyReactiveDictionary<MapStagesEnum, MapStageData> MapStages => _mapStages;


        #endregion
        
        #region Setters

        public void UpdateMapStageData(MapStageData mapStageData)
        {
            _mapStages[mapStageData.MapStageType] = mapStageData;
        }
        public MapStageData GetMapStageData(MapStagesEnum mapStageType)
        {
            return _mapStages.TryGetValue(mapStageType, out var data) ? data : CreateNewMapStageData(mapStageType);
        }

        private MapStageData CreateNewMapStageData(MapStagesEnum mapStageType)
        {
            MapStageData playerTankData = new MapStageData
            {
                MapStageType = mapStageType
            };
            _mapStages.Add(mapStageType, playerTankData);

            return playerTankData;
        }
        #endregion

        #region Storage
        public MapStagesData GetMapStageData()
        {
            return new MapStagesData
            {
                MapStageBlockDatas = _mapStages.Values.ToList(),
            };
        }

        public void SetMapStageData(MapStagesData mapStagesData)
        {
            foreach (var playerCharacterData in mapStagesData.MapStageBlockDatas)
            {
                _mapStages.Add(playerCharacterData.MapStageType, playerCharacterData);
            }
        }
        
        
        public void SetAndInitEmptyMapStageData(MapStagesData mapStagesData)
        {
            mapStagesData.MapStageBlockDatas = new List<MapStageData>();
            SetMapStageData(mapStagesData);
        }
        #endregion
    }
}