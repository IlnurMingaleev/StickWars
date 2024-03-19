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
using Views.Units.Units;

namespace Models.Merge
{
    public class Item : MonoBehaviour
    {
        public int id;
        public Slot parentSlot;

        public Transform unitParent;
        public PlayerView unitGameObject;
        public BasePlayerUnit BasePlayerUnit;

        private static List<ProjectileView> _projectiles = new();
        public void Init(int id, Slot slot, IPlayerUnitsBuilder playerBuilder)
        {
            this.id = id;
            this.parentSlot = slot;
            unitGameObject = playerBuilder.InstantiateUnit((PlayerUnitTypeEnum) id, unitParent,parentSlot.slotType);
            
        }

       
        private void OnDestroy()
        {
            Destroy(unitGameObject);
        }
    }
}
