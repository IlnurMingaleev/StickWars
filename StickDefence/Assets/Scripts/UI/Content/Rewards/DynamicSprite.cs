using System;
using SO.Visual;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Content.Rewards
{
    public class DynamicSprite : MonoBehaviour
    {
        [SerializeField] protected Image _image;
        [SerializeField] protected DynamicSpritesSO _sprites;
        
        public int Index { get; private set; }
        
        public void UpdateSprite(float value)
        {
            if (_sprites.DynamicSprites.Count == 0) throw new Exception("No ranges were found.");

            int i = _sprites.DynamicSprites.Count - 1;
            while (i > 0)
            {
                if (_sprites.DynamicSprites[i].Value < value)
                    break;
                --i;
            }

            _image.SetSpriteSync(_sprites.DynamicSprites[i].Sprite);
            Index = i;
        }
    }
}