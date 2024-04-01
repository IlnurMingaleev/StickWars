using System;
using System.Collections.Generic;
using Enums;
using Models.Move;

namespace Models.SO.Core
{
    [Serializable]
    public struct GroupUnitsConfig
    {
        public List<GroupUnitConfig> Units;
        public int WaveUnitsQty;
    }

    [Serializable]
    public struct GroupUnitConfig
    {
        public UnitTypeEnum UnitType;
        public int Count;
        public float Delay;
        public UnitMovementTypeEnum MovementType;
    }
}