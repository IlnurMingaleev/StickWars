using UnityEngine;

namespace Models.Merge
{
    public class Item : MonoBehaviour
    {
        public int id;
        public Slot parentSlot;

        public Transform unitParent;
        public GameObject unitGameObject;

        public void Init(int id, Slot slot)
        {
            this.id = id;
            this.parentSlot = slot;
            unitGameObject = Instantiate(Utils.GetUnitById(id), unitParent);
        }

        private void OnDestroy()
        {
            Destroy(unitGameObject);
        }
    }
}
