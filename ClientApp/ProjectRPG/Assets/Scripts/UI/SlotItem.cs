using UnityEngine;

public class SlotItem : MonoBehaviour
{
    // 아이템 Id.
    private int RefId;

    // 아이템 수량
    private int Count;

    protected void Awake()
    {
    }

    public void OnMouseDown()
    {
        // 아이템 수량이 0보다 크고, 소모성 아이템인 경우.
        if (Count > 0)
        {
            if (Inventory.Instance.IsUseable(RefId))
            {
                Count--;
                Debug.Log($"Use item. refId: {RefId}");

                Inventory.Instance.UseItem(RefId);
            }
        }
    }

    /// <summary>
    /// Add item count.
    /// </summary>
    public void AddItemCount(int addCount)
    {
        Count += addCount;
    }

    /// <summary>
    /// Get item refId.
    /// </summary>
    public int GetRefId()
    {
        return RefId;
    }

    /// <summary>
    /// Get item count.
    /// </summary>
    public int GetCount()
    {
        return RefId;
    }
}
