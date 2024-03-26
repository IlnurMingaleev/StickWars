using System;
using Enums;
using UnityEngine;

namespace Models.SO.Visual
{
    [Serializable]
    public struct RewardConfigIconModel
    {
        [field: SerializeField] public RewardIconTypeEnum RewardIconType { get; private set; }
        [field: SerializeField] public Sprite Image { get; private set; }
    }
}