using System.Collections.Generic;
using Enums;
using Models.Battle;
using Models.Timers;
using Models.Units;
using Models.Units.Units;
using TonkoGames.Controllers.Core;
using TonkoGames.Sound;
using TonkoGames.StateMachine.Enums;
using UnityEngine;
using Views.Projectiles;
using Views.Units.Fortress;
using Views.Units.Units;

namespace Models.Merge
{
    public class Item : MonoBehaviour
    {
        public int id;
        public Slot parentSlot;
        public PlayerUnitTypeEnum UnitTypeEnum;

        public Transform unitParent;
        public PlayerViewTwo unitGameObject;
        public BasePlayerUnit BasePlayerUnit;

        private static List<ProjectileView> _projectiles = new();
        public void Init(int id, Slot slot, IPlayerUnitsBuilderTwo playerBuilder)
        {
            this.id = id;
            this.parentSlot = slot;
            UnitTypeEnum = (PlayerUnitTypeEnum) id;
            unitGameObject = playerBuilder.InitStageLoadBattle((PlayerUnitTypeEnum) id, unitParent,parentSlot.slotType);
            
        }

       
        private void OnDestroy()
        {
            Destroy(unitGameObject);
        }
    }
}
