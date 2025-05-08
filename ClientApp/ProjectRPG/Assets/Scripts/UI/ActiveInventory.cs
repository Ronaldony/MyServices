using GameCore;
using Network;
using UnityEngine;

public class ActiveInventory : Singleton<ActiveInventory>
{
    private int _activeSlotIndexNum = 0;

    private NetPacketManager _netPacketManager;
    private PlayerControls _playerControls;

    protected override void Awake()
    {
        base.Awake();

        _netPacketManager = NetPacketManager.Instance;
        _playerControls = new PlayerControls();
    }

    private void Start()
    {
        _playerControls.Inventory.keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    public void EquipStartingWeapon()
    {
        ToggleActiveHighlight(0);
    }

    private void ToggleActiveSlot(int numValue)
    {
        ToggleActiveHighlight(numValue - 1);
    }

    private void ToggleActiveHighlight(int indexNum)
    {
        _activeSlotIndexNum = indexNum;

        foreach (Transform child in this.transform)
        {
            // �� Child�� Active ������Ʈ inactive.
            child.GetChild(0).gameObject.SetActive(false);
        }

        // Active index inventory.
        this.transform.GetChild(_activeSlotIndexNum).GetChild(0).gameObject.SetActive(true);
         
        ChangeActiveWeapon();
    }

    private void ChangeActiveWeapon()
    {
        // ���� ���� �ı�.
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }

        Transform childTransform = transform.GetChild(_activeSlotIndexNum);
        InventorySlot slot = childTransform.GetComponentInChildren<InventorySlot>();
        WeaponInfo weaponInfo = slot.GetWeaponInfo();

        // Check weapon.
        if (!weaponInfo)
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }

        Debug.Log($"_activeSlotIndexNum: {_activeSlotIndexNum}");

        // TB_Weapon.
        var tbWeapon = TDBManagement.Instance.Select<TB_Weapon>(d => d.Id == (_activeSlotIndexNum + 1));
        if (null == tbWeapon)
        {
            Debug.LogError("No weapon!");
            return;
        }

        // New weapon.
        GameObject weaponToSpawn = weaponInfo.weaponPrefab;
        var newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform);

        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }
}
