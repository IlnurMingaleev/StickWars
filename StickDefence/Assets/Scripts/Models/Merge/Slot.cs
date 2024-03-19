using System.Collections;
using System.Collections.Generic;
using Enums;
using Models.Battle;
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

        [Inject] private ConfigManager _configManager;
        [Inject] private ITimerService _timerService;
        [Inject] private ISoundManager _soundManager;

        public void CreateItem(int id, IPlayerUnitsBuilder _playerUnitBuilder) 
        {
            var itemGO = Instantiate((GameObject)Resources.Load("Prefabs/Item"));
        
            itemGO.transform.SetParent(this.transform);
            itemGO.transform.localPosition = Vector3.zero;
            itemGO.transform.localScale = Vector3.one;

            currentItem = itemGO.GetComponent<Item>();
            currentItem.Init(id, this,_playerUnitBuilder);

            ChangeStateTo(SlotState.Full);
        }

        private void ChangeStateTo(SlotState targetState)
        {
            state = targetState;
        }

        public void ItemGrabbed()
        {
            Destroy(currentItem.gameObject);
            ChangeStateTo(SlotState.Empty);
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