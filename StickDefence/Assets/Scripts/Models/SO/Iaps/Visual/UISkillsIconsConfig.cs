using System;
using Enums;
using UnityEngine;

namespace Models.SO.Visual
{
    [Serializable]
    public struct UISkillsIconsConfig
    {
        [field: SerializeField] public SkillTypesEnum SkillType { get; private set; }
        [field: SerializeField] public Sprite Value { get; private set; }
    }
}