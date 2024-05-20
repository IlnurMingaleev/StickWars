using Enums;
using Models.Battle;
using UnityEngine;
using Views.Units.Fortress;

namespace Models.Merge
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private Transform _unitParent; 
        
        private Slot _parentSlot;
        private PlayerUnitView _unitGameObject;

        public Slot ParentSlot => _parentSlot;
        
        public int Id { get; private set; }
        public PlayerUnitTypeEnum UnitTypeEnum { get; private set; }
        public void Init(int id, Slot slot, IPlayerUnitsBuilder playerBuilder)
        {
            Id = id;
            _parentSlot = slot;
            UnitTypeEnum = (PlayerUnitTypeEnum) id;
            _unitGameObject = playerBuilder.InitPlayerUnit((PlayerUnitTypeEnum) id, _unitParent, _parentSlot.SlotType);
        }
       
        private void OnDestroy()
        {
            Destroy(_unitGameObject);
        }
    }
}
