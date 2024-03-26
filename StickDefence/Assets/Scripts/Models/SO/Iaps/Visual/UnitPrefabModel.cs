using System;
using Enums;
using UnityEngine;

namespace Models.SO.Visual
{
    [Serializable]
    public struct UnitPrefabModel
    {
        [field: SerializeField] public UnitTypeEnum UnitType { get; private set; }
        [field: SerializeField] public GameObject GO { get; private set; }
    }

    [Serializable]
    public struct PlayerPrefabModel
    {
        [field: SerializeField] public PlayerUnitTypeEnum UnitType { get; private set; }
        [field: SerializeField] public GameObject GO { get; private set; } 
        [field: SerializeField] public Sprite uiIcon;
    }
}