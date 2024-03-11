using System.Collections.Generic;
using Enums;
using Models.SO.Visual;
using UnityEngine;
using Views.Units.Fortress;

namespace SO.Visual
{
    [CreateAssetMenu(fileName = "PrefabsUnitsSO", menuName = "MyAssets/Config/PrefabsUnitsSO", order = 7)]
    public class PrefabsUnitsSO : ScriptableObject
    {
        [SerializeField] private List<UnitPrefabModel> _unitPrefabsModels;
        [field: SerializeField] public FortressView FortressView { get; private set; }
        
        private Dictionary<UnitTypeEnum, GameObject> _dictionaryUnitPrefabs =
            new Dictionary<UnitTypeEnum, GameObject>();
        
        public IReadOnlyDictionary<UnitTypeEnum, GameObject> UnitPrefabs  => _dictionaryUnitPrefabs;
        
        public void Init()
        {
            foreach (var unitPrefab in _unitPrefabsModels)
                _dictionaryUnitPrefabs.Add(unitPrefab.UnitType, unitPrefab.GO);
        }
    }
}