using System.Collections.Generic;
using UnityEngine;

namespace Models.Merge
{
    public interface IPlaceableUnit
    {
        void PlaceDefinedItem(int level);
    }

    public class MergeController : MonoBehaviour, IPlaceableUnit
    {
        public static MergeController instance;

        public Slot[] slots;

        private Vector3 _target;
        private ItemInfo carryingItem;

        private Dictionary<int, Slot> slotDictionary;

        private void Awake() {
            instance = this;
            Utils.InitResources();
        }

        private void Start() 
        {
            slotDictionary = new Dictionary<int, Slot>();

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].id = i;
                slotDictionary.Add(i, slots[i]);
            }
        }

        //handle user input
        private void Update() 
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                SendRayCast();
            }

            if (UnityEngine.Input.GetMouseButton(0) && carryingItem)
            {
                OnItemSelected();
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                //Drop item
                SendRayCast();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                PlaceRandomItem();
            }
        }

        void SendRayCast()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition), Vector2.zero);
 
            //we hit something
            if(hit.collider != null)
            {
                //we are grabbing the item in a full slot
                if (hit.collider.gameObject.TryGetComponent(out Slot slot))
                {
                    if (slot.state == SlotState.Full && carryingItem == null)
                    {
                        var itemGO = (GameObject) Instantiate(Resources.Load("Prefabs/ItemDummy"));
                        itemGO.transform.position = slot.transform.position;
                        itemGO.transform.localScale = Vector3.one * 2;

                        carryingItem = itemGO.GetComponent<ItemInfo>();
                        carryingItem.InitDummy(slot.id, slot.currentItem.id);

                        slot.ItemGrabbed();

                    }

                    //we are dropping an item to empty slot
                    else if (slot.state == SlotState.Empty && carryingItem != null)
                    {
                        slot.CreateItem(carryingItem.itemId);
                        Destroy(carryingItem.gameObject);
                    }

                    //we are dropping to full
                    else if (slot.state == SlotState.Full && carryingItem != null)
                    {
                        //check item in the slot
                        if (slot.currentItem.id == carryingItem.itemId)
                        {
                            print("merged");
                            OnItemMergedWithTarget(slot.id);
                        }
                        else
                        {
                            SwitchItems(slot);
                        }
                    }
                }

            }
            else
            {
                if (!carryingItem)
                {
                    return;
                }
                OnItemCarryFail();
            }
        }

        private void SwitchItems(Slot slot)
        {
            var targetSlot = GetSlotById(slot.id);
            var startSlot = GetSlotById(carryingItem.slotId);
            startSlot.CreateItem(targetSlot.currentItem.id);
            Destroy(targetSlot.currentItem.gameObject);
            targetSlot.CreateItem(carryingItem.itemId);
            Destroy(carryingItem.gameObject);
        }

        void OnItemSelected()
        {
            _target = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            _target.z = 0;
            var delta = 10 * Time.deltaTime;
        
            delta *= Vector3.Distance(transform.position, _target);
            carryingItem.transform.position = Vector3.MoveTowards(carryingItem.transform.position, _target, delta);
        }

        void OnItemMergedWithTarget(int targetSlotId)
        {
            var slot = GetSlotById(targetSlotId);
            Destroy(slot.currentItem.gameObject);
        
            slot.CreateItem(carryingItem.itemId + 1);

            Destroy(carryingItem.gameObject);
        }

        void OnItemCarryFail()
        {
            var slot = GetSlotById(carryingItem.slotId);
            slot.CreateItem(carryingItem.itemId);
            Destroy(carryingItem.gameObject);
        }

        void PlaceRandomItem()
        {
            if (AllSlotsOccupied())
            {
                Debug.Log("No empty slot available!");
                return;
            }

            var rand = UnityEngine.Random.Range(0, slots.Length - 5);
            var slot = GetSlotById(rand);

            while (slot.state == SlotState.Full)
            {
                rand = UnityEngine.Random.Range(0, slots.Length- 5);
                slot = GetSlotById(rand);
            }

            slot.CreateItem(0);
        }

        public void PlaceDefinedItem(int level)
        {
            if (AllSlotsOccupied())
            {
                Debug.Log("No empty slot available!");
                return;
            }
        
            Slot slot = null;
            for (int index = 0; index < slots.Length - 5 ; index++)
            {
                slot = GetSlotById(index);
                if (slot.state == SlotState.Empty)
                {
                    break;
                }

            }
            slot.CreateItem(level);
        }


        bool AllSlotsOccupied()
        {
            for (var i = 0; i < slots.Length - 5; i++)
            {
                if (slots[i].state == SlotState.Empty)
                {
                    return false;
                }
            }
            //no slot empty 
            return true;
        }

        Slot GetSlotById(int id)
        {
            return slotDictionary[id];
        }
    }
}