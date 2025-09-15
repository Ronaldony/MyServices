using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
{
    // 아이템 Id.
    private int _refId;

    // 아이템 수량
    private int _count;

    private int _useDelaySec = 1;
    private bool _canUseItem = true;

    protected void Awake()
    {
        UpdateSlotCountText();
    }

    public void UseItem()
    {
        if (!_canUseItem)
        {
            return;
        }

        // 아이템 수량이 0보다 크고, 소모성 아이템인 경우.
        if (_count > 0)
        {
            if (Inventory.Instance.IsUseable(_refId))
            {
                _count--;
                Debug.Log($"Use item. refId: {_refId}");

                Inventory.Instance.UseItem(_refId);

                UpdateSlotCountText();
            }

            // Use delay routine.
            StartCoroutine(UseDelayRoutine());
        }
    }

    /// <summary>
    /// Set slot.
    /// </summary>
    public void SetSlot(int refId, int count)
    {
        _refId = refId;
        _count = count;

        UpdateSlotCountText();
    }

    /// <summary>
    /// Add item count.
    /// </summary>
    public void PlusItem(int addCount)
    {
        _count += addCount;

        Debug.Log($"Add item count. refId: {_refId}");

        UpdateSlotCountText();
    }

    /// <summary>
    /// Get item refId.
    /// </summary>
    public int GetRefId()
    {
        return _refId;
    }

    /// <summary>
    /// Get item count.
    /// </summary>
    public int GetCount()
    {
        return _refId;
    }

    /// <summary>
    /// Update slot count.
    /// </summary>
    private void UpdateSlotCountText()
    {
        var text = GetComponentInChildren<Text>();
        text.text = _count.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UseItem();
    }

    private IEnumerator UseDelayRoutine()
    {
        _canUseItem = false;

        yield return new WaitForSeconds(_useDelaySec);

        _canUseItem = true;
    }
}
