using Enums;
using TonkoGames.Controllers.Core;
using UnityEngine;

namespace Models.Merge
{
    public class ItemInfo : MonoBehaviour 
    {
        public int slotId;
        public int itemId;

        public Transform unitParent;
        public GameObject unitGameObject;

        public void InitDummy(int slotId, int itemId, ConfigManager configManager) 
        {
            this.slotId = slotId;
            this.itemId = itemId;
            unitGameObject =
                Instantiate(configManager.StickmanUnitsSO.DictionaryStickmanConfigs[(PlayerUnitTypeEnum) itemId].stickmanGO,unitParent);
        }

        private void OnDestroy()
        {
            Destroy(unitGameObject);
        }
    }
}