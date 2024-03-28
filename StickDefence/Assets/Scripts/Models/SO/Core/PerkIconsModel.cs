using System;
using Enums;
using UnityEngine;

namespace Models.SO.Core
{
    [Serializable]

    public struct PerkIconsModel
    {
        public PerkTypesEnum PerkType;
        public Sprite PerkIcon;
    }
}