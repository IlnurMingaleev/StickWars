using System.Collections.Generic;
using Enums;
using Models.SO.Core;
using Models.SO.Visual;
using UnityEngine;
using Views.Units.Fortress;

namespace SO.Visual
{
    [CreateAssetMenu(fileName = "PrefabsUnitsSO", menuName = "MyAssets/Config/PrefabsUnitsSO", order = 7)]
    public class PrefabsUnitsSO : ScriptableObject
    {
        [SerializeField] private List<UnitPrefabModel> _unitPrefabsModels;
        [SerializeField] private List<PlayerPrefabModel> _playerUnitPrefabsModels;
        [SerializeField] private List<PerkIconsModel> _perkIconModels;
        [field: SerializeField] public FortressView FortressView { get; private set; }
        [field: SerializeField] public FortressView FortressViewMilitary { get; private set; }
        
        private Dictionary<UnitTypeEnum, GameObject> _dictionaryUnitPrefabs =
            new Dictionary<UnitTypeEnum, GameObject>();
        private Dictionary<PlayerUnitTypeEnum, PlayerPrefabModel> _dictionaryPlayerUnitPrefabs =
            new Dictionary<PlayerUnitTypeEnum, PlayerPrefabModel>();

        private Dictionary<PerkTypesEnum, PerkIconsModel> _dictionaryPerkIcons =
            new Dictionary<PerkTypesEnum, PerkIconsModel>();

        public IReadOnlyDictionary<UnitTypeEnum, GameObject> UnitPrefabs  => _dictionaryUnitPrefabs;
        public IReadOnlyDictionary<PlayerUnitTypeEnum, PlayerPrefabModel> PlayerUnitPrefabs  => _dictionaryPlayerUnitPrefabs;

        public IReadOnlyDictionary<PerkTypesEnum, PerkIconsModel> PerkIcons => _dictionaryPerkIcons;
        public void Init()
        {
            _dictionaryUnitPrefabs.Clear();
            _dictionaryPlayerUnitPrefabs.Clear();
            _dictionaryPerkIcons.Clear();
            foreach (var unitPrefab in _unitPrefabsModels)
                _dictionaryUnitPrefabs.Add(unitPrefab.UnitType, unitPrefab.GO);

            foreach (var playerUnitPrefabs in _playerUnitPrefabsModels)
                _dictionaryPlayerUnitPrefabs.Add(playerUnitPrefabs.UnitType, playerUnitPrefabs);

            foreach (var perkIconModel in _perkIconModels)
                _dictionaryPerkIcons.Add(perkIconModel.PerkType,perkIconModel);
                
        }
    }
}