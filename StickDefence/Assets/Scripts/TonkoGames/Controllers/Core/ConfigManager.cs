﻿using Models.SO.Core;
using SO.Core;
using SO.Iaps;
using SO.Visual;
using UnityEngine;

namespace TonkoGames.Controllers.Core
{
    public class ConfigManager : MonoBehaviour
    {
        [field: SerializeField] public SpritesSO SpritesSo { get; private set; }
        [field: SerializeField] public RewardsConstSO RewardsConstSO { get; private set; }
        [field: SerializeField] public PumpingConfigSO PumpingConfigSo { get; private set; }
        [field: SerializeField] public UnitsStatsSO UnitsStatsSo { get; private set; }
        [field: SerializeField] public IAPSO IapSO { get; private set; }
        [field: SerializeField] public MapStageSO MapStageSO { get; private set; }
        [field: SerializeField] public PrefabsUnitsSO PrefabsUnitsSO { get; private set; }
        [field: SerializeField] public StickmanUnitStatsSO StickmanUnitsSO { get; private set; }
        [field: SerializeField] public BoosterSpritesSO BoostSpritesSO { get; private set;}

        private void Awake()
        {
            SpritesSo.Init();
            PumpingConfigSo.Init();
            UnitsStatsSo.Init();
            PrefabsUnitsSO.Init();
            IapSO.Init();
            MapStageSO.Init();
            BoostSpritesSO.Init();
        }
    }
}