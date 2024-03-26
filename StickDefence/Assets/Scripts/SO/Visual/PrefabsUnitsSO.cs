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
        [field: SerializeField] public FortressView FortressView { get; private set; }
        
        private Dictionary<UnitTypeEnum, GameObject> _dictionaryUnitPrefabs =
            new Dictionary<UnitTypeEnum, GameObject>();
        private Dictionary<PlayerUnitTypeEnum, PlayerPrefabModel> _dictionaryPlayerUnitPrefabs =
            new Dictionary<PlayerUnitTypeEnum, PlayerPrefabModel>();
        
        
        public IReadOnlyDictionary<UnitTypeEnum, GameObject> UnitPrefabs  => _dictionaryUnitPrefabs;
        public IReadOnlyDictionary<PlayerUnitTypeEnum, PlayerPrefabModel> PlayerUnitPrefabs  => _dictionaryPlayerUnitPrefabs;
        public void Init()
        {
            _dictionaryUnitPrefabs.Clear();
            _dictionaryPlayerUnitPrefabs.Clear();
            foreach (var unitPrefab in _unitPrefabsModels)
                _dictionaryUnitPrefabs.Add(unitPrefab.UnitType, unitPrefab.GO);

            foreach (var playerUnitPrefabs in _playerUnitPrefabsModels)
            {
                _dictionaryPlayerUnitPrefabs.Add(playerUnitPrefabs.UnitType, playerUnitPrefabs);   
            }
            
        }
    }
}