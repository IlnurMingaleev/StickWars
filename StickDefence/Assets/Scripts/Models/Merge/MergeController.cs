using System.Collections.Generic;
using Enums;
using I2.Loc;
using Models.Battle;
using Models.DataModels;
using Models.Timers;
using TonkoGames.Controllers.Core;
using UI.Content.Battle;
using UI.UIManager;
using UI.Windows;
using UnityEngine;
using VContainer;

namespace Models.Merge
{
    public interface IPlaceableUnit
    {
        void PlaceDefinedItem(int level);
    }

    public class MergeController : MonoBehaviour, IPlaceableUnit
    {
        [SerializeField] private PlayerUnitsBuilderTwo _playerUnitsBuilder;
        private Camera _mainCamera;
        public static MergeController instance;
        
        public Slot[] slots;

        private Vector3 _target;
        private ItemInfo carryingItem;

        private Dictionary<int, Slot> slotDictionary;
        private IPlayerUnitsBuilderTwo _playerBuilder;
        [Inject] private ConfigManager _configManager;
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private IWindowManager _windowManager;
        [Inject] private ITimerService _timerService;

        private void Awake()
        {
          
            instance = this;
            _playerUnitsBuilder.GetComponent<IPlayerUnitsBuilder>();
            Utils.InitResources();
        }

        public void Init()
        {
            foreach (var slot in slots)
            {
                PlayerUnitTypeEnum playerUnitType = _dataCentralService.MapStageDataModel.SlotItems[slot.SlotIdType].PlayerUnitType; 
                if(playerUnitType == PlayerUnitTypeEnum.None)
                     continue;
                else
                     slot.CreateItem((int)playerUnitType,_playerBuilder);

            }
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            _playerBuilder = _playerUnitsBuilder.GetComponent<IPlayerUnitsBuilderTwo>();
            slotDictionary = new Dictionary<int, Slot>();

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetId(i);
                slotDictionary.Add(i, slots[i]);
            }
            Init();
        }

        //handle user input
        private void Update() 
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                SendRayCast();
            }

            if ((UnityEngine.Input.GetMouseButton(0)) && carryingItem)
            {
                OnItemSelected();
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                //Drop item
                SendRayCast();
            }
            
        }

        void SendRayCast()
        {
            RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition), Vector2.zero);
 
            //we hit something
            if(hit.collider != null)
            {
                if(hit.collider.gameObject.TryGetComponent(out TrashBinSlot trashBin))
                {
                    if (carryingItem != null)
                    {
                        Destroy(carryingItem.gameObject);
                        SetGrabbedFlag();
                    }
                }
                //we are grabbing the item in a full slot
                if (hit.collider.gameObject.TryGetComponent(out Slot slot))
                {
                    if (slot.State == SlotState.Full && carryingItem == null)
                    {
                        var itemGO = (GameObject) Instantiate(Resources.Load("Prefabs/ItemDummy"));
                        itemGO.transform.position = slot.transform.position;
                        itemGO.transform.localScale = Vector3.one;

                        carryingItem = itemGO.GetComponent<ItemInfo>();
                        carryingItem.InitDummy(slot.Id, slot.CurrentItem.Id,_configManager);

                        slot.ItemGrabbed();

                    }

                    //we are dropping an item to empty slot
                    else if (slot.State == SlotState.Empty && carryingItem != null)
                    {
                        slot.CreateItem(carryingItem.itemId,_playerBuilder);
                        SetGrabbedFlag();
                        Destroy(carryingItem.gameObject);
                    }

                    //we are dropping to full
                    else if (slot.State == SlotState.Full && carryingItem != null)
                    {
                        //check item in the slot
                        if (slot.CurrentItem.Id == carryingItem.itemId)
                        {
                            print("merged");
                            OnItemMergedWithTarget(slot.Id);
                        }
                        else
                        {
                            SwitchItems(slot);
                        }
                    }
                }

            }
            else
            {
                if (!carryingItem)
                {
                    return;
                }
                OnItemCarryFail();
            }
            
        }

        private void SwitchItems(Slot slot)
        {
            var targetSlot = GetSlotById(slot.Id);
            var startSlot = GetSlotById(carryingItem.slotId);
            startSlot.CreateItem(targetSlot.CurrentItem.Id, _playerBuilder);
            targetSlot.DestroyItem();
            targetSlot.CreateItem(carryingItem.itemId,_playerBuilder);
            SetGrabbedFlag();
            Destroy(carryingItem.gameObject);
        }

        void OnItemSelected()
        {
            _target = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            _target.z = 0;
            var delta = 10 * Time.deltaTime;
        
            delta *= Vector3.Distance(transform.position, _target);
            carryingItem.transform.position = Vector3.MoveTowards(carryingItem.transform.position, _target, delta);
        }

        void OnItemMergedWithTarget(int targetSlotId)
        {
            if ((PlayerUnitTypeEnum) carryingItem.itemId == _dataCentralService.PumpingDataModel.MaxStickmanLevel.Value)
            {
                _dataCentralService.PumpingDataModel.UpgradeMaxStickmanLevel();
                _dataCentralService.SaveFull();
            } 
            var slot = GetSlotById(targetSlotId);
            slot.DestroyItem();
            slot.CreateItem(carryingItem.itemId + 1, _playerBuilder);
            SetGrabbedFlag();
            Destroy(carryingItem.gameObject);
        }

        private void SetGrabbedFlag()
        {
            var prevSlot = GetSlotById(carryingItem.slotId);
            prevSlot.SetIsItemGrabbed(false);
        }

        void OnItemCarryFail()
        {
            var slot = GetSlotById(carryingItem.slotId);
            slot.CreateItem(carryingItem.itemId,_playerBuilder);
            SetGrabbedFlag();
            Destroy(carryingItem.gameObject);
        }

        void PlaceRandomItem()
        {
            if (AllSlotsOccupied())
            {
                Debug.Log("No empty slot available!");
                return;
            }

            var rand = UnityEngine.Random.Range(0, slots.Length - 5);
            var slot = GetSlotById(rand);

            while (slot.State == SlotState.Full)
            {
                rand = UnityEngine.Random.Range(0, slots.Length- 5);
                slot = GetSlotById(rand);
            }

            slot.CreateItem(0,_playerBuilder);
        }

        public void PlaceDefinedItem(int level)
        {
            if (AllSlotsOccupied())
            {
                Debug.Log("No empty slot available!");
                _windowManager.GetWindow<PopupMessageWindow>().Init(ScriptLocalization.Messages.WarningTitle, ScriptLocalization.Messages.SlotsFull);
                _windowManager.Show<PopupMessageWindow>(); 
                return;
            }
        
            Slot slot = null;
            for (int index = 0; index < slots.Length - 5 ; index++)
            {
                slot = GetSlotById(index);
                if (slot.State == SlotState.Empty && (!slot.IsItemGrabbed))
                {
                    break;
                }

            }
            slot.CreateItem(level,_playerBuilder);
        }


       public  bool AllSlotsOccupied()
        {
            for (var i = 0; i < slots.Length - 5; i++)
            {
                if (slots[i].State == SlotState.Empty && (!slots[i].IsItemGrabbed))
                {
                    return false;
                }
            }
            //no slot empty 
            return true;
        }

        Slot GetSlotById(int id)
        {
            return slotDictionary[id];
        }

        #region AutoMerge

        public void AutoMerge()
        {
            for (int slotIndex = 0; slotIndex < slots.Length - 5; slotIndex++)
            {
                if (slots[slotIndex].State == SlotState.Full)
                {
                    for (int innerSlotIndex = slotIndex + 1; innerSlotIndex < slots.Length - 5; innerSlotIndex++)
                    {

                        if (slots[innerSlotIndex].State == SlotState.Full)
                        {
                            PlayerUnitTypeEnum unitType = slots[slotIndex].CurrentItem.UnitTypeEnum;
                            PlayerUnitTypeEnum innerUnitType= slots[innerSlotIndex].CurrentItem.UnitTypeEnum;
                            if( unitType == innerUnitType && unitType != PlayerUnitTypeEnum.Twenty)
                            {
                                slots[slotIndex].DestroyItem(); 
                                slots[innerSlotIndex].DestroyItem();
                                slots[innerSlotIndex].CreateItem((int)unitType + 1, _playerBuilder);
                                return;
                            }
                        }

                    }
                }
            }
        }

        
        #endregion

        private void CreateSlot(int id, IPlayerUnitsBuilderTwo playerUnitBuilder)
        {
            
        }
    }
}