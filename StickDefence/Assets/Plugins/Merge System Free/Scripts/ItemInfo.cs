using UnityEngine;

public class ItemInfo : MonoBehaviour 
{
    public int slotId;
    public int itemId;

    public Transform unitParent;
    public GameObject unitGameObject;

    public void InitDummy(int slotId, int itemId) 
    {
        this.slotId = slotId;
        this.itemId = itemId;
        unitGameObject = Instantiate(Utils.GetUnitById(itemId),unitParent);
    }
}