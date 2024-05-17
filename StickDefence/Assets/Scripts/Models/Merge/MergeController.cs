using System;
using System.Collections.Generic;
using Enums;
using I2.Loc;
using Models.Battle;
using Models.DataModels;
using Models.Timers;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using UI.Content.Battle;
using UI.UIManager;
using UI.Windows;
using UniRx;
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
        [SerializeField] private ItemInfo _itemDummyPrefab;
        private Camera _mainCamera;
        public static MergeController instance;

        public Slot[] slots;

        private Vector3 _target;
        private ItemInfo _carryingItem;

        private Dictionary<int, Slot> slotDictionary;
        [Inject] private ConfigManager _configManager;
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private IWindowManager _windowManager;
        [Inject] private ICoreStateMachine _coreStateMachine;

        private CompositeDisposable _moveDisposable = new CompositeDisposable();
        private CompositeDisposable _mainDisposable = new CompositeDisposable();
        private void Awake()
        {

            instance = this;
            Utils.InitResources();
        }

        private void Init()
        {
            SubscribeToMouseEvents();
            foreach (var slot in slots)
            {
                var playerUnitType = _dataCentralService.MapStageDataModel.SlotItems[slot.SlotIdType].PlayerUnitType;

                if (playerUnitType == PlayerUnitTypeEnum.None)
                    continue;
                CreateSlotItem(slot, (int) playerUnitType);
            }

            _coreStateMachine.RunTimeStateMachine.RunTimeState.Subscribe(state => OnRuntimeStateChange(state))
                .AddTo(_mainDisposable);
        }

        private void OnRuntimeStateChange(RunTimeStateEnum state)
        {
            if (state == RunTimeStateEnum.Pause)
            {
                if(_carryingItem != null)OnItemCarryFail();
                UnsubscribeFromMouseEvents();
            }
            else if (state == RunTimeStateEnum.Play)
            {
                SubscribeToMouseEvents();
            }
        }

        private void SubscribeToMouseEvents()
        {
            foreach (var slot in slots)
            {
                slot.OnSlotClick += OnSlotClick;
                slot.OnSlotUp += OnReleaseSlotClick;
            }
        }

        private void UnsubscribeFromMouseEvents()
        {
            foreach (var slot in slots)
            {
                slot.OnSlotClick -= OnSlotClick;
                slot.OnSlotUp -= OnReleaseSlotClick;
            }
        }


        private void Start()
        {
            _mainCamera = Camera.main;
            slotDictionary = new Dictionary<int, Slot>();

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetId(i);
                slotDictionary.Add(i, slots[i]);
            }

            Init();
        }
        
        void OnReleaseSlotClick(Slot mouseClickSlot)
        {
            Slot slot = mouseClickSlot;
            RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition), Vector2.zero);
           if(hit.collider!= null)
           {
               if (hit.collider.TryGetComponent(out Slot raycastSlot))
               {
                   slot = raycastSlot;
               }

           }
            if (slot.SlotType == SlotTypeEnum.TrashBin)
            {
                if (_carryingItem != null)
                {
                    Destroy(_carryingItem.gameObject);
                    SetGrabbedFlag();
                }
            }
            else
            {
                if (slot.State == SlotState.Empty && _carryingItem != null)
                {
                    CreateSlotItem(slot, _carryingItem.itemId);
                    SetGrabbedFlag();
                    Destroy(_carryingItem.gameObject);
                }
                else if (slot.State == SlotState.Full && _carryingItem != null)
                {
                    //check item in the slot
                    if (slot.CurrentItem.Id == _carryingItem.itemId)
                    {
                        print("merged");
                        OnItemMergedWithTarget(slot.Id);
                    }
                    else
                    {
                        SwitchItems(slot);
                    }
                }
                else
                {
                    if (!_carryingItem)
                    {
                        return;
                    }

                    OnItemCarryFail();
                }
                _moveDisposable.Clear();
            }
        }

        void OnSlotClick(Slot slot)
        {
            if (slot.State == SlotState.Full && _carryingItem == null)
            {
                _carryingItem = Instantiate(_itemDummyPrefab, transform);
                _carryingItem.transform.position = slot.transform.position;
                _carryingItem.transform.localScale = Vector3.one;

                _carryingItem.InitDummy(slot.Id, slot.CurrentItem.Id, _configManager);

                slot.ItemGrabbed();
                Observable.EveryUpdate().Subscribe(_ => { OnItemSelected();}).AddTo(_moveDisposable);
            }
        }

        private void SwitchItems(Slot slot)
        {
            var targetSlot = GetSlotById(slot.Id);
            var startSlot = GetSlotById(_carryingItem.slotId);
            CreateSlotItem(startSlot, targetSlot.CurrentItem.Id);
            targetSlot.DestroyItem();
            CreateSlotItem(targetSlot, _carryingItem.itemId);
            SetGrabbedFlag();
            Destroy(_carryingItem.gameObject);
        }

        void OnItemSelected()
        {
            if(_carryingItem == null) return;
            _target = _mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            _target.z = 0;
            var delta = 10 * Time.deltaTime;

            delta *= Vector3.Distance(transform.position, _target);
            _carryingItem.transform.position = Vector3.MoveTowards(_carryingItem.transform.position, _target, delta);
        }

        void OnItemMergedWithTarget(int targetSlotId)
        {
            if ((PlayerUnitTypeEnum) _carryingItem.itemId == _dataCentralService.PumpingDataModel.MaxStickmanLevel.Value)
            {
                if(_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value != PlayerUnitTypeEnum.Twenty) _dataCentralService.PumpingDataModel.UpgradeMaxStickmanLevel();
                _dataCentralService.SaveFull();
            }

            var slot = GetSlotById(targetSlotId);
            slot.DestroyItem();
            CreateSlotItem(slot, _carryingItem.itemId + 1);
            SetGrabbedFlag();
            Destroy(_carryingItem.gameObject);
        }

        private void SetGrabbedFlag()
        {
            var prevSlot = GetSlotById(_carryingItem.slotId);
            prevSlot.SetIsItemGrabbed(false);
        }

        void OnItemCarryFail()
        {
            var slot = GetSlotById(_carryingItem.slotId);
            CreateSlotItem(slot, _carryingItem.itemId);
            SetGrabbedFlag();
            Destroy(_carryingItem.gameObject);
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
                rand = UnityEngine.Random.Range(0, slots.Length - 5);
                slot = GetSlotById(rand);
            }

            CreateSlotItem(slot, 0);
        }

        public void PlaceDefinedItem(int level)
        {
            if (AllSlotsOccupied())
            {
                Debug.Log("No empty slot available!");
                _windowManager.GetWindow<PopupMessageWindow>().Init(ScriptLocalization.Messages.WarningTitle,
                    ScriptLocalization.Messages.SlotsFull);
                _windowManager.Show<PopupMessageWindow>();
                return;
            }

            Slot slot = null;
            for (int index = 0; index < slots.Length - 5; index++)
            {
                slot = GetSlotById(index);
                if (slot.State == SlotState.Empty && (!slot.IsItemGrabbed))
                {
                    break;
                }

            }

            CreateSlotItem(slot, level);
        }


        public bool AllSlotsOccupied()
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
                            PlayerUnitTypeEnum innerUnitType = slots[innerSlotIndex].CurrentItem.UnitTypeEnum;
                            if (unitType == innerUnitType && unitType != PlayerUnitTypeEnum.Twenty)
                            {
                                slots[slotIndex].DestroyItem();
                                slots[innerSlotIndex].DestroyItem();
                                CreateSlotItem(slots[innerSlotIndex], (int) unitType + 1);
                                return;
                            }
                        }

                    }
                }
            }
        }

        #endregion

        private void CreateSlotItem(Slot slot, int id)
        {
            slot.CreateItem(id, _playerUnitsBuilder, _dataCentralService);
        }

        private void OnDisable()
        {
            if(_carryingItem != null)OnItemCarryFail();
            UnsubscribeFromMouseEvents();
            _moveDisposable.Clear();
        }
    }
}
