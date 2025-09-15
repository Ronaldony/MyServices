using GameCore;
using GameCore.Datas.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Singleton<Inventory>
{
    /// <summary>
    /// ItemPrefab.
    /// </summary>
    [Serializable]
    public class ItemPrefab
    {
        public int RefId;

        public Sprite ItemSprite;
    }

    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private GameObject _inventoryObj;
    [SerializeField] private GameObject _itemsObj;
    [SerializeField] private ItemPrefab[] _itemSprites;

    private readonly Dictionary<int, InventorySlot> _slots;

    private bool _showInventory = false;

    public Inventory()
    {
        _slots = new Dictionary<int, InventorySlot>();
    }

    protected override void Awake()
    {
        base.Awake();
    }
    /// <summary>
    /// Toggle inventory.
    /// </summary>
    public void Toggle()
    {
        Debug.Log($"ToggleInventory");

        if (_showInventory)
        {
            // Off inventory.
            _showInventory = false;
            _inventoryObj.SetActive(false);
        }
        else
        {
            // On inventory.
            _showInventory = true;
            _inventoryObj.SetActive(true);
        }
    }

    /// <summary>
    /// Is show inventory.
    /// </summary>
    public bool IsShowInventory()
    {
        return _showInventory;
    }

    /// <summary>
    /// Check item is useable.
    /// </summary>
    public bool IsUseable(int refId)
    {
        var findTBItem = TDBManagement.Instance.GetById<TB_Item>(refId);

        if (null == findTBItem)
        {
            return false;
        }

        // 소모 재료가 아닌경우, 소모 실패.
        if (findTBItem.ItemType != Type_Item.Consume)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Use item.
    /// </summary>
    public void UseItem(int refId)
    {
        var tbItem = TDBManagement.Instance.GetById<TB_Item>(refId);

        // Consume 1.
        var tbConsume = TDBManagement.Instance.GetById<TB_ItemConsume>(tbItem.Consume_Ref1);

        if (tbConsume != null)
        {
            ConsumeItem(tbConsume, tbItem.Consume_Value1);
        }

        // Consume 2.
        tbConsume = TDBManagement.Instance.GetById<TB_ItemConsume>(tbItem.Consume_Ref2);

        if (tbConsume != null)
        {
            ConsumeItem(tbConsume, tbItem.Consume_Value2);
        }
    }

    private void ConsumeItem(TB_ItemConsume tbItemConsume, int consumeValue)
    {
        switch (tbItemConsume.ConsumeType)
        {
            case Type_ItemConsume.HP_Plus:
                PlayerHealth.Instance.HealPlayer(consumeValue);
                break;
            case Type_ItemConsume.Stamina_Plus:
                Stamina.Instance.RefreshStamina(consumeValue);
                break;
        }
    }

    /// <summary>
    /// Pickup item.
    /// </summary>
    public void PickupItem(DTO_Item item)
    {
        var tbItem = TDBManagement.Instance.GetById<TB_Item>(item.RefId);

        if (tbItem == null)
        {
            Debug.LogError($"Pick up item error. refId: {item.RefId}");
            return;
        }

        if (_slots.ContainsKey(item.RefId))
        {
            // 인벤토리 슬롯에 이미 생성된 경우.
            var slot = _slots[item.RefId];

            slot.PlusItem(item.Count);

        }
        else if (tbItem.ItemType == Type_Item.Gold)
        {
            // Update gold score.
            EconomyManager.Instance.PlusGold(item.Count);
        }
        else
        {
            // 아이템 이미지.
            var findItemSprite = _itemSprites.FirstOrDefault(d => d.RefId == item.RefId);

            if (null == findItemSprite)
            {
                Debug.LogError($"Can't find item sprite. refId: {item.RefId}");
                return;
            }

            // 인벤토리 슬롯 생성.
            var slotPrefab = Instantiate(_slotPrefab, transform.position, Quaternion.identity, _itemsObj.transform);
            var slotImage = slotPrefab.GetComponentsInChildren<Image>().FirstOrDefault(d => d.name == "Item");

            if (null == slotImage)
            {
                Debug.LogError($"Can't find item image.");
                return;
            }

            slotImage.sprite = findItemSprite.ItemSprite;

            // 슬롯 정보.
            var slot = slotPrefab.GetComponent<InventorySlot>();
            slot.SetSlot(item.RefId, item.Count);

            _slots.Add(item.RefId, slot);
        }
    }
}
