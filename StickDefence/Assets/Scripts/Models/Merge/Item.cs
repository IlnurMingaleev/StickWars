using Enums;
using Models.Battle;
using UnityEngine;
using Views.Units.Fortress;

namespace Models.Merge
{
    public class Item : MonoBehaviour
    {
        private Slot _parentSlot;
        private Transform _unitParent;
        private PlayerViewTwo _unitGameObject;
        
        public int Id { get; private set; }
        public PlayerUnitTypeEnum UnitTypeEnum { get; private set; }
        public void Init(int id, Slot slot, IPlayerUnitsBuilderTwo playerBuilder)
        {
            Id = id;
            _parentSlot = slot;
            UnitTypeEnum = (PlayerUnitTypeEnum) id;
            _unitGameObject = playerBuilder.InitStageLoadBattle((PlayerUnitTypeEnum) id, _unitParent, _parentSlot.slotType);
        }
       
        private void OnDestroy()
        {
            Destroy(_unitGameObject);
        }
    }
}
