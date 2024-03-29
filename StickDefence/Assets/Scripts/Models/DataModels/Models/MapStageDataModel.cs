using System.Collections.Generic;
using System.Linq;
using Enums;
using Models.DataModels.Data;
using Models.Merge;
using UniRx;

namespace Models.DataModels.Models
{
    public interface IMapStageDataModel
    {
        #region Fields
        IReadOnlyReactiveDictionary<MapStagesEnum, MapStageData> MapStages { get; } 
        IReadOnlyReactiveDictionary<SlotIdTypeEnum, SlotItemData> SlotItems { get; }

        #endregion
        
        #region Setters
        void UpdateMapStageData(MapStageData mapStageData);
        MapStageData GetMapStageData(MapStagesEnum mapStageType); 
        void UpdateSlotItemData(SlotItemData slotItemData);
        SlotItemData GetSlotItemData(SlotIdTypeEnum slotIdType);

        #endregion
    } 
    public class MapStageDataModel : IMapStageDataModel
    {
        #region Fields

        private ReactiveDictionary<MapStagesEnum, MapStageData> _mapStages =
            new ReactiveDictionary<MapStagesEnum, MapStageData>();

        private ReactiveDictionary<SlotIdTypeEnum, SlotItemData> _slotItems =
            new ReactiveDictionary<SlotIdTypeEnum, SlotItemData>(); 
        public IReadOnlyReactiveDictionary<MapStagesEnum, MapStageData> MapStages => _mapStages;
        
        public IReadOnlyReactiveDictionary<SlotIdTypeEnum, SlotItemData> SlotItems => _slotItems;

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

        public void UpdateSlotItemData(SlotItemData slotItemData)
        {
            _slotItems[slotItemData.SlotIdTypeEnum] = slotItemData;
        }

        public SlotItemData GetSlotItemData(SlotIdTypeEnum slotIdType)
        {
            return _slotItems.TryGetValue(slotIdType, out var data) ? data : CreateNewSlotItemData(slotIdType);
        }
        

        private SlotItemData CreateNewSlotItemData( SlotIdTypeEnum slotIdTypeEnum)
        {
            SlotItemData slotItemData = new SlotItemData
            {
                SlotIdTypeEnum = slotIdTypeEnum,
                PlayerUnitType =PlayerUnitTypeEnum.None
            };
            _slotItems.Add(slotIdTypeEnum, slotItemData);
            return slotItemData;
        }

        #endregion

        #region Storage
        public MapStagesData GetMapStageData()
        {
            return new MapStagesData
            {
                MapStageBlockDatas = _mapStages.Values.ToList(),
                SlotItemDatas = _slotItems.Values.ToList(),
            };
        }
        
        public void SetMapStageData(MapStagesData mapStagesData)
        {
            foreach (var playerCharacterData in mapStagesData.MapStageBlockDatas)
            {
                _mapStages.Add(playerCharacterData.MapStageType, playerCharacterData);
            }
            foreach (var slotItem in mapStagesData.SlotItemDatas)
            {
                _slotItems.Add(slotItem.SlotIdTypeEnum,slotItem);
            }
        }
        
        
        public void SetAndInitEmptyMapStageData(MapStagesData mapStagesData)
        {
            mapStagesData.MapStageBlockDatas = new List<MapStageData>();

            List<SlotItemData> slotItems = new List<SlotItemData>();
            for (int index = 0; index < 16; index++)
            {
                slotItems.Add(new SlotItemData
                {
                    SlotIdTypeEnum = (SlotIdTypeEnum) index,
                    PlayerUnitType = PlayerUnitTypeEnum.None,
                });
            }

            mapStagesData.SlotItemDatas = slotItems;
            SetMapStageData(mapStagesData);
        }
        
        #endregion
    }
}