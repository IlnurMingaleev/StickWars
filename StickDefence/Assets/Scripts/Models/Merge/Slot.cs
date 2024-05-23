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
        [field: SerializeField] private int _sortingOrder;
        public int Id { get; private set; }
        public Item CurrentItem { get; private set; }
        public SlotState State { get; private set; }

        public bool IsItemGrabbed { get; private set; }
        public int SortingOrder => _sortingOrder;
        private IDataCentralService _dataCentralService;
        

        public event Action<Slot> OnSlotClick;
        public event Action<Slot> OnSlotUp;
        

        public void CreateItem(int id, IPlayerUnitsBuilder playerUnitBuilder, IDataCentralService dataCentralService) 
        {
            _dataCentralService = dataCentralService;
            
            CurrentItem = Instantiate(_itemPrefab, transform);
            
            CurrentItem.Init(id, this, playerUnitBuilder);
            _dataCentralService.MapStageDataModel.UpdateSlotItemData(
               new SlotItemData()
               {
                   SlotIdTypeEnum = SlotIdType,
                   PlayerUnitType = (PlayerUnitTypeEnum)id,
               });
            _dataCentralService.SaveFull();    
            ChangeStateTo(SlotState.Full);
        }
        public void MoveItem(int id, Item carryingItem, IDataCentralService dataCentralService) 
        {
            _dataCentralService = dataCentralService;

            CurrentItem = carryingItem;
            CurrentItem.gameObject.transform.SetParent(transform);
            CurrentItem.gameObject.transform.position = transform.position; 
            
            
            CurrentItem.SetProperties (CurrentItem,this);
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

        public Item PassItem()
        {
            Item tmpItem = null;
            if(CurrentItem != null && State== SlotState.Empty) tmpItem = CurrentItem;
            ZeroSlotData();
            return tmpItem;
        }

        public void RecieveItem(Item item)
        {
            
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

        public void ZeroSlotData()
        {
            ChangeStateTo(SlotState.Empty);
            CurrentItem = null;
            _dataCentralService.MapStageDataModel.UpdateSlotItemData(
                new SlotItemData()
                {
                    SlotIdTypeEnum = SlotIdType,
                    PlayerUnitType = PlayerUnitTypeEnum.None,
                });
            _dataCentralService.SaveFull();  
        }
        public void ChangeStateTo(SlotState targetState)
        {
            State = targetState;
        }

        public void ItemGrabbed()
        {
            IsItemGrabbed = true;
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

        public void OnPointerDown()
        {
            Debug.Log("OnMouseDown");
            if(CurrentItem != null)
                CurrentItem.ActivateOutline();
            OnSlotClick?.Invoke(this);
        }

        public void OnPointerUp()
        {
            Debug.Log("OnMouseUp");
            if(CurrentItem != null)
                CurrentItem.DeactivateOutline();
            OnSlotUp?.Invoke(this);
        }
    }

    public enum SlotState
    {
        Empty,
        Full
    }
}