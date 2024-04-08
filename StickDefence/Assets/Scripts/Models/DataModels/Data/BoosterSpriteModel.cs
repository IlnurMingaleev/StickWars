using System;
using Enums;
using UnityEngine;

namespace Models.DataModels.Data
{
    [Serializable]
    public struct BoosterSpriteModel
    {
        [field: SerializeField] public BoosterTypeEnum BoosterType { get; set; }
        [field: SerializeField] public Sprite BoosterSprite { get; set; }
    }
}