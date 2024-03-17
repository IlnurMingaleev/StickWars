using Enums;
using TonkoGames.Controllers.Core;
using UnityEngine;

namespace Models.Merge
{
    public class Item : MonoBehaviour
    {
        public int id;
        public Slot parentSlot;

        public Transform unitParent;
        public GameObject unitGameObject;

        public void Init(int id, Slot slot, ConfigManager configManager)
        {
            this.id = id;
            this.parentSlot = slot;
            unitGameObject = Instantiate(configManager.StickmanUnitsSO.DictionaryStickmanConfigs[(PlayerUnitTypeEnum) id].stickmanGO,unitParent);
        }

        private void OnDestroy()
        {
            Destroy(unitGameObject);
        }
    }
}
