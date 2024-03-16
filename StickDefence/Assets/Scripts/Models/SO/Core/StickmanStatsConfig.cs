using System;
using Enums;
using UnityEngine;

namespace Models.SO.Core
{
    [Serializable]
    public struct StickmanStatsConfig
    {
        public PlayerUnitTypeEnum UnitType;
        public GameObject stickmanGO;
        public Sprite uiIcon;
        public int Level;
        public int Damage;
        public float Reloading;
        public int Price;
        

    }
}