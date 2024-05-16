using System;
using Enums;
using Models.Battle;
using Models.DataModels;
using Models.DataModels.Data;
using UnityEngine;

namespace Models.Merge
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private Item _itemPrefab;
        [field: SerializeField] public SlotTypeEnum SlotType { get; private set; }
        [field: SerializeField] public SlotIdTypeEnum SlotIdType { get; private set; }
        
        public int Id { get; private set; }
        public Item CurrentItem { get; private set; }
        public SlotState State { get; private set; }

        public bool IsItemGrabbed { get; private set; }
        
        private IDataCentralService _dataCentralService;

        public Action<Slot> OnSlotClick;
        public Action<Slot> OnSlotUp;

        public void CreateItem(int id, IPlayerUnitsBuilderTwo playerUnitBuilder, IDataCentralService dataCentralService,
            Action<Slot> onSlotClick = null, Action<Slot> onSlotUp = null) 
        {
            _dataCentralService = dataCentralService;
            
            CurrentItem = Instantiate(_itemPrefab, transform);
            
            CurrentItem.Init(id, this, playerUnitBuilder);
            OnSlotClick = onSlotClick;
            OnSlotUp = onSlotUp;
            
            _dataCentralService.MapStageDataModel.UpdateSlotItemData(
               new SlotItemData()
               {
                   SlotIdTypeEnum = SlotIdType,
                   PlayerUnitType = (PlayerUnitTypeEnum)id,
               });
            _dataCentralService.SaveFull();    
            ChangeStateTo(SlotState.Full);
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public void SetIsItemGrabbed(bool value)
        {
            IsItemGrabbed = value;
        }

        public void DestroyItem()
        {
            ChangeStateTo(SlotState.Empty);
            if(CurrentItem != null)
                Destroy(CurrentItem.gameObject);
          
            _dataCentralService.MapStageDataModel.UpdateSlotItemData(
                new SlotItemData()
                {
                    SlotIdTypeEnum = SlotIdType,
                    PlayerUnitType = PlayerUnitTypeEnum.None,
                });
            _dataCentralService.SaveFull();  
        }

        private void ChangeStateTo(SlotState targetState)
        {
            State = targetState;
        }

        public void ItemGrabbed()
        {
            IsItemGrabbed = true;
           DestroyItem();
        }

        private void ReceiveItem(int id)
        {
            switch (State)
            {
                case SlotState.Empty: 

                   // CreateItem(id);
                    ChangeStateTo(SlotState.Full);
                    break;

                case SlotState.Full: 
                    if (CurrentItem.Id == id)
                    {
                        //Merged
                        Debug.Log("Merged");
                    }
                    else
                    {
                        //Push item back
                        Debug.Log("Push back");
                    }
                    break;
            }
        }

        private void OnMouseDown()
        {
            Debug.Log("OnMouseDown");
            OnSlotClick?.Invoke(this);
        }

        private void OnMouseUp()
        {
            Debug.Log("OnMouseUp");
            OnSlotUp?.Invoke(this);
        }
    }

    public enum SlotState
    {
        Empty,
        Full
    }
}