using GameCore;
using GameCore.Datas.Items;

public class Inventory : Singleton<Inventory>
{
    private PlayerControls _playerControls;

    private bool _showInventory = false;

    protected override void Awake()
    {
        base.Awake();

        _playerControls = new PlayerControls();
    }

    private void Start()
    {
        _playerControls.Inventory.Toggle.performed += ctx => ToggleInventory();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    /// <summary>
    /// Toggle inventory.
    /// </summary>
    private void ToggleInventory()
    {
        if (_showInventory)
        {
            // Off inventory.
        }
        else
        {
            // On inventory.
        }
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
        //case PickUpType.GoldCoin:
        //EconomyManager.Instance.UpdateCurrentGold();
        //break;
    }

    /// <summary>
    /// Pickup item.
    /// </summary>
    public void PickupItem(DTO_Item item)
    {
        var tbItem = TDBManagement.Instance.GetById<TB_Item>(item.RefId);

        foreach (var slotItem in GetComponentsInChildren<SlotItem>())
        {
            if (slotItem.GetRefId() == item.RefId)
            {
                slotItem.AddItemCount(item.Count);

                // Update gold score.
                if (tbItem.ItemType == Type_Item.Gold)
                {
                    EconomyManager.Instance.UpdateCurrentGold(slotItem.GetCount());
                }

                break;
            }
        }
    }
}
