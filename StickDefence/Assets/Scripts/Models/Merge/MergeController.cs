using System.Collections.Generic;
using Enums;
using I2.Loc;
using Models.Battle;
using Models.DataModels;
using TonkoGames.Controllers.Core;
using TonkoGames.Sound;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
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
        [SerializeField] private PlayerUnitsBuilder _playerUnitsBuilder;
        //[SerializeField] private Item _itemDummyPrefab;
        private Camera _mainCamera;
        public static MergeController instance;

        public Slot[] slots;

        private Vector3 _target;
        private Item _carryingItem;
        private BottomPanelWindow _bottomPanelWindow;

        private Dictionary<int, Slot> slotDictionary;
        [Inject] private ConfigManager _configManager;
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private IWindowManager _windowManager;
        [Inject] private ICoreStateMachine _coreStateMachine;
        [Inject] private ISoundManager _soundManager;

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

        private void OnReleaseSlotClick(Slot mouseClickSlot)
        {
            var slot = mouseClickSlot;
            RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition), Vector3.zero);
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out Slot raycastSlot))
                {
                    slot = raycastSlot;
                }else if (hit.collider.TryGetComponent(out Item raycastItem))
                {
                    slot = raycastItem.ParentSlot;
                }
            }



            if (slot.SlotType == SlotTypeEnum.TrashBin)
            {
                if (_carryingItem != null)
                {
                    SetGrabbedFlag();
                    Destroy(_carryingItem.gameObject);
                }
            }
            else
            {
                if (_carryingItem != null)
                {
                    switch (slot.State)
                    {
                        case SlotState.Empty:
                            MoveSlotItem(slot,_carryingItem.ItemId,_carryingItem);
                            break;
                        case SlotState.Full:
                            if (_carryingItem.SlotId == slot.Id)
                            {
                                OnItemCarryFail();   
                            }
                            else if (slot.CurrentItem.Id == _carryingItem.ItemId)
                                OnItemMergedWithTarget(slot.Id);
                            else
                                SwitchItems(slot);
                            break;
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
                if(_carryingItem != null) _carryingItem.DeactivateOutline();;
                _carryingItem = null;
                _moveDisposable.Clear();
            }
            _soundManager.PlayPutOneShot();
        }

        void OnSlotClick(Slot slot)
        {
            if (slot.State == SlotState.Full && _carryingItem == null)
            {
               _carryingItem = slot.CurrentItem;
               slot.ZeroSlotData();

                slot.ItemGrabbed();
                Observable.EveryUpdate().Subscribe(_ => OnItemSelected()).AddTo(_moveDisposable);
                if(_carryingItem != null) _carryingItem.ActivateOutline();
                _soundManager.PlayPickUpOneShot();
            }
        }

        private void SwitchItems(Slot slot)
        {
            var targetSlot = GetSlotById(slot.Id);
            var startSlot = GetSlotById(_carryingItem.SlotId);
            MoveSlotItem(startSlot,targetSlot.CurrentItem.Id, targetSlot.CurrentItem);
            targetSlot.ZeroSlotData();
            MoveSlotItem(targetSlot,_carryingItem.ItemId,_carryingItem);
           
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
            
            if ((PlayerUnitTypeEnum) _carryingItem.ItemId == PlayerUnitTypeEnum.Twenty)
            {
                _windowManager.GetWindow<BottomPanelWindow>().ShowMaxLevelReachedWarning();
                OnItemCarryFail();
                return;
            }
            if ((PlayerUnitTypeEnum) _carryingItem.ItemId == _dataCentralService.PumpingDataModel.MaxStickmanLevel.Value)
            {
                _dataCentralService.PumpingDataModel.UpgradeMaxStickmanLevel();
                _dataCentralService.SaveFull();
            }
            var slot = GetSlotById(targetSlotId);
            slot.DestroyItem();
            SetGrabbedFlag();
            var tmpId = _carryingItem.ItemId;
            Destroy(_carryingItem.gameObject);
            CreateSlotItem(slot, tmpId+ 1);
            if (_bottomPanelWindow == null) _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
            _bottomPanelWindow.UpdateQuickBuyBtn();

        }

        private void SetGrabbedFlag()
        {
            var prevSlot = GetSlotById(_carryingItem.SlotId);
            prevSlot.SetIsItemGrabbed(false);
        }

        void OnItemCarryFail()
        {
            var slot = GetSlotById(_carryingItem.SlotId);
            MoveSlotItem(slot,_carryingItem.ItemId, _carryingItem);
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
        private void MoveSlotItem(Slot targetSlot, int id, Item item)
        {
            SetGrabbedFlag();
            targetSlot.MoveItem(id,item,_dataCentralService);
        }
        private void OnDisable()
        {
            if(_carryingItem != null)OnItemCarryFail();
            UnsubscribeFromMouseEvents();
            _moveDisposable.Clear();
        }
    }
}
