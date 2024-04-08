using System.Collections.Generic;
using Enums;
using Models.DataModels.Data;
using UnityEngine;

namespace SO.Visual
{
    [CreateAssetMenu(fileName = "BoosterSpritesSO", menuName = "MyAssets/Config/BoosterSpritesSO", order = 15)]
    public class BoosterSpritesSO: ScriptableObject
    {
        [SerializeField] public List<BoosterSpriteModel> _boosterSprites;

        private Dictionary<BoosterTypeEnum,Sprite> _dictionaryBoosterSprites =
            new Dictionary<BoosterTypeEnum,Sprite>();       
        public Dictionary<BoosterTypeEnum, Sprite> BoosterSprites => _dictionaryBoosterSprites;
        
        public void Init()
        { 
            _dictionaryBoosterSprites.Clear();
            foreach (var unitPrefab in _boosterSprites)
                _dictionaryBoosterSprites.Add(unitPrefab.BoosterType, unitPrefab.BoosterSprite);
        }
    }
}