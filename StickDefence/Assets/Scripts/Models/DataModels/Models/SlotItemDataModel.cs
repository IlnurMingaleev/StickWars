using System.Collections.Generic;
using System.Linq;
using Enums;
using Models.DataModels.Data;
using Models.Merge;
using UniRx;

namespace Models.DataModels.Models
{
    public interface ISlotItemDataModel
    {
        IReadOnlyReactiveDictionary<SlotIdTypeEnum, SlotItemData> SlotItems { get; }
        void UpdateSlotItemData(SlotItemData slotItemData);
        SlotItemData GetSlotItemData(SlotIdTypeEnum slotIdType);
    }

    public class SlotItemDataModel : ISlotItemDataModel
    {
        #region Fields
        private ReactiveDictionary<SlotIdTypeEnum, SlotItemData> _slotItems =
            new ReactiveDictionary<SlotIdTypeEnum, SlotItemData>(); 
        public IReadOnlyReactiveDictionary<SlotIdTypeEnum, SlotItemData> SlotItems => _slotItems;
        #endregion


        #region Setters

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
       public SlotItemsData GetSlotItemData()
       {
           return new SlotItemsData
           {
               slotItems = _slotItems.Values.ToList(),
           };
       }

       public void SetSlotItemData(SlotItemsData slotItemsData)
       {
           foreach (var slotItem in slotItemsData.slotItems)
           {
               _slotItems.Add(slotItem.SlotIdTypeEnum,slotItem);
           }
       }

       public void SetAndInitEmptySlotItemData(SlotItemsData slotItemsData)
       {
           slotItemsData.slotItems = new List<SlotItemData>();
           SetSlotItemData(slotItemsData);
       }
       
       #endregion

    }
}