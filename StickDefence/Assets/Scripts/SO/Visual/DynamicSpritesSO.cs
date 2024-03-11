using System.Collections.Generic;
using Models.SO.Visual;
using UnityEngine;

namespace SO.Visual
{
    [CreateAssetMenu(fileName = "DynamicSpritesSO", menuName = "MyAssets/Sprites/DynamicSpritesSO", order = 3)]
    public class DynamicSpritesSO : ScriptableObject
    {
        [SerializeField] private List<DynamicSpriteConfig> _dynamicSprites;

        public IReadOnlyList<DynamicSpriteConfig> DynamicSprites => _dynamicSprites;
    }
}