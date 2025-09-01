using GameCore.Datas.Items;
using System;
using System.Linq;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private DropItem[] _dropItems;

    /// <summary>
    /// Drop item.
    /// </summary>
    public void DropItem(long dropId, DTO_Item dtoItem)
    {
        // Drop item.
        var dropItem = _dropItems.FirstOrDefault(d => d.RefId == dtoItem.RefId);
        
        if (null !=  dropItem)
        {
            var itemObject = Instantiate(dropItem.GameObject, transform.position, Quaternion.identity);
            var pickup = itemObject.GetComponent<Pickup>();

            pickup.SetDropItem(dropId, dtoItem);

            Debug.Log($"Drop drop item. refId: {dtoItem.RefId}");
        }
        else
        {
            Debug.Log($"Fail to drop item. refId: {dtoItem.RefId}");
        }
    }
}

[Serializable]
public class DropItem
{
    public int RefId;

    public GameObject GameObject;
}
