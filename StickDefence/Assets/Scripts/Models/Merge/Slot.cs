using System.Collections;
using System.Collections.Generic;
using Enums;
using Models.Battle;
using Models.DataModels;
using Models.DataModels.Data;
using Models.Timers;
using TonkoGames.Controllers.Core;
using TonkoGames.Sound;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

namespace Models.Merge
{
    public class Slot : MonoBehaviour
    {
        public int id;
        public Item currentItem;
        public SlotState state = SlotState.Empty;
        public SlotTypeEnum slotType = SlotTypeEnum.None;
        public SlotIdTypeEnum slotIdType = SlotIdTypeEnum.None;

        [Inject] private ConfigManager _configManager;
        [Inject] private ITimerService _timerService;
        [Inject] private ISoundManager _soundManager;
        [Inject] private IDataCentralService _dataCentralService;

        public void CreateItem(int id, IPlayerUnitsBuilderTwo _playerUnitBuilder) 
        {
            var itemGO = Instantiate((GameObject)Resources.Load("Prefabs/Item"));
        
            itemGO.transform.SetParent(this.transform);
            itemGO.transform.localPosition = Vector3.zero;
            itemGO.transform.localScale = Vector3.one;

            currentItem = itemGO.GetComponent<Item>();
            currentItem.Init(id, this,_playerUnitBuilder);
            _dataCentralService.MapStageDataModel.UpdateSlotItemData(
               new SlotItemData()
               {
                   SlotIdTypeEnum = slotIdType,
                   PlayerUnitType = (PlayerUnitTypeEnum)id,
               });
            _dataCentralService.SaveFull();    
            ChangeStateTo(SlotState.Full);
        }

        public void DestroyItem()
        {
            ChangeStateTo(SlotState.Empty);
            Destroy(currentItem.gameObject);
          
            _dataCentralService.MapStageDataModel.UpdateSlotItemData(
                new SlotItemData()
                {
                    SlotIdTypeEnum = slotIdType,
                    PlayerUnitType = (PlayerUnitTypeEnum)id,
                });
            _dataCentralService.SaveFull();  
            
        }

        private void ChangeStateTo(SlotState targetState)
        {
            state = targetState;
        }

        public void ItemGrabbed()
        {
            DestroyItem();
        }

        private void ReceiveItem(int id)
        {
            switch (state)
            {
                case SlotState.Empty: 

                   // CreateItem(id);
                    ChangeStateTo(SlotState.Full);
                    break;

                case SlotState.Full: 
                    if (currentItem.id == id)
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
    }

    public enum SlotState
    {
        Empty,
        Full
    }
}