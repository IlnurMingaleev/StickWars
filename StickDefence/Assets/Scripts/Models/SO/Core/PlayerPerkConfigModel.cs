﻿using System;
using Enums;

namespace Models.SO.Core
{
    [Serializable]
    public struct PlayerPerkConfigModel
    {
        public PerkTypesEnum PerkType;
        public float BaseValue;
        public float AdditionalValue;
        public int LevelCount;
        public int BaseCost;
        public int AdditionalCost;
        public CurrencyTypeEnum CurrencyType;
    }
}