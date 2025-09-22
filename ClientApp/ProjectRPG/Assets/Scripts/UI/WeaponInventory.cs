using GameCore;
using GameCore.GamePacket;
using GamePackets;
using UnityEngine;

public class WeaponInventory : Singleton<WeaponInventory>
{
    private int _activeSlotIndexNum = 0;

    private PlayerControls _playerControls;

    protected override void Awake()
    {
        base.Awake();

        _playerControls = new PlayerControls();
    }

    private void Start()
    {
        _playerControls.Weapons.keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    public void ToggleActiveSlot(int numValue)
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
        // Destroy.
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }

        Transform childTransform = transform.GetChild(_activeSlotIndexNum);
        WeaponSlot slot = childTransform.GetComponentInChildren<WeaponSlot>();
        WeaponInfo weaponInfo = slot.GetWeaponInfo();

        // Check weapon.
        if (!weaponInfo)
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }

        // TB_Weapon.
        var tbWeapon = TDBManagement.Instance.GetById<TB_Weapon>(_activeSlotIndexNum + 1);
        if (null == tbWeapon)
        {
            Debug.LogError("No weapon!");
            return;
        }

        // Refresh WeaponInfo.
        weaponInfo.weaponId = tbWeapon.Id;

        // Send switch weapon.
        GamePacketManager.Instance.Send<GamePacketCS_SwitchWeapon>(GamePacketCode.CS_SwitchWeapon, new GamePacketCS_SwitchWeapon
        {
            WeaponId = weaponInfo.weaponId
        });

        // New weapon.
        GameObject weaponToSpawn = weaponInfo.weaponPrefab;
        var newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform);

        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }
}
