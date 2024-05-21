using System;
using Enums;
using I2.Loc;
using Models.Battle;
using TonkoGames.StateMachine;
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
        private PlayerUnitView _unitGameObject;

        public Slot ParentSlot => _parentSlot;
        private Action<Slot> OnItemPointerDown;
        public int Id { get; private set; }
        public PlayerUnitTypeEnum UnitTypeEnum { get; private set; }
        public void Init(int id, Slot slot, IPlayerUnitsBuilder playerBuilder)
        {
            Id = id;
            _parentSlot = slot;
            UnitTypeEnum = (PlayerUnitTypeEnum) id;
            _unitGameObject = playerBuilder.InitPlayerUnit((PlayerUnitTypeEnum) id, _unitParent, _parentSlot.SlotType);
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
            Destroy(_unitGameObject);
        }

        public void ActivateOutline()
        {
            _unitGameObject.SetStickmanMaterial(_outlineMaterial);
        }

        public void DeactivateOutline()
        {
            _unitGameObject.SetStickmanMaterial(_coloredMaterail);
        }
    }
}
