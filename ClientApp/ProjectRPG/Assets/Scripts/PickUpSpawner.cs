using GameCore;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoin, healthGlobe, staminaGlobe;

    private Type_Item _itemType;
    
    /// <summary>
    /// Drop items.
    /// </summary>
    public void DropItems()
    {
        switch (_itemType)
        {
            case Type_Item.Gold:
                Instantiate(goldCoin, transform.position, Quaternion.identity);
                break;
            case Type_Item.Stamina:
            Instantiate(staminaGlobe, transform.position, Quaternion.identity);
                break;
            case Type_Item.Heart:
            Instantiate(healthGlobe, transform.position, Quaternion.identity);
                break;
        }
    }

    /// <summary>
    /// Initialize spawn.
    /// </summary>
    public void Initialize_Spawn(Type_Item itemType)
    {
        _itemType = itemType;
    }
}
