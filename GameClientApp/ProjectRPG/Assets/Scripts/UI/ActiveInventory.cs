using UnityEngine;

public class ActiveInventory : MonoBehaviour
{
    private int activeSlotIndexNum = 0;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerControls.Inventory.keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());

        ToggleActiveHighlight(0);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void ToggleActiveSlot(int numValue)
    {
        ToggleActiveHighlight(numValue - 1);
    }

    private void ToggleActiveHighlight(int indexNum)
    {
        activeSlotIndexNum = indexNum;

        foreach (Transform child in this.transform)
        {
            // 각 Child의 Active 오브젝트 inactive.
            child.GetChild(0).gameObject.SetActive(false);
        }

        // Active index inventory.
        this.transform.GetChild(activeSlotIndexNum).GetChild(0).gameObject.SetActive(true);
         
        ChangeActiveWeapon();
    }

    private void ChangeActiveWeapon()
    {
        // 이전 무기 파괴.
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }

        // Inventory 슬롯이 있는 경우, 해당 스롯에 무기 비우기. (장착 효과)
        if (!transform.GetChild(activeSlotIndexNum).GetComponentInChildren<InventorySlot>())
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }

        // WeaponInfo 프리팹 불러오기.
        var weaponToSpawn = transform
            .GetChild(activeSlotIndexNum)
            .GetComponentInChildren<InventorySlot>()
            .GetWeaponInfo().weaponPrefab;

        // 새로운 무기
        var newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity);

        // 무기 위치 조정.
        newWeapon.transform.parent = ActiveWeapon.Instance.transform;

        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }
}
