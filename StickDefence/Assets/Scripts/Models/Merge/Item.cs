using System;
using Enums;
using Models.Battle;
using Models.Fortress;
using UnityEngine;
using Views.Units.Fortress;

namespace Models.Merge
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private Transform _unitParent;
        [SerializeField] private Material _outlineMaterial;
        [SerializeField] private Material _coloredMaterail;
        
        private Slot _parentSlot;
        private PlayerUnitView _playerUnitView;
        private PlayerUnitModel _playerUnitModel;

        public Slot ParentSlot => _parentSlot;
        private Action<Slot> OnItemPointerDown;
        
        public int SlotId =>_parentSlot.Id;
        public int ItemId => Id;
        public int Id { get; private set; }
        public PlayerUnitTypeEnum UnitTypeEnum { get; private set; }
        public void Init(int id, Slot slot, IPlayerUnitsBuilder playerBuilder)
        {
            Id = id;
            _parentSlot = slot;
            UnitTypeEnum = (PlayerUnitTypeEnum) id;
            (PlayerUnitView view, PlayerUnitModel model) playerUnit= playerBuilder.InitPlayerUnit((PlayerUnitTypeEnum) id, _unitParent, _parentSlot.SlotType);
            _playerUnitModel = playerUnit.model;
            _playerUnitView = playerUnit.view;
            SetPlayerSortOrder(_parentSlot.SortingOrder);
        }
        public void SetProperties(Item item, Slot slot)
        {
            Id = item.ItemId;
            _parentSlot = slot;
            UnitTypeEnum = (PlayerUnitTypeEnum) item.ItemId;
            if(_playerUnitModel != null) _playerUnitModel.SetParentSlotType(_parentSlot.SlotType);
            SetPlayerSortOrder(_parentSlot.SortingOrder);
        }

        public void OnPointerDown()
        {
            Debug.Log("Item Pointer Down");
            if(_parentSlot != null) _parentSlot.OnPointerDown();
        }
        public void OnPointerUp()
        {
            Debug.Log("Item Pointer Down");
            if(_parentSlot != null) _parentSlot.OnPointerUp();
        }

        private void OnDestroy()
        {
            Destroy(_playerUnitView);
        }

        public void ActivateOutline()
        {
            _playerUnitView.SetStickmanMaterial(_outlineMaterial);
        }

        public void DeactivateOutline()
        {
            _playerUnitView.SetStickmanMaterial(_coloredMaterail);
        }

        public void SetPlayerSortOrder(int sortOrder)
        {
            _playerUnitView.SetSortingOrder(sortOrder);
        }
    }
}
