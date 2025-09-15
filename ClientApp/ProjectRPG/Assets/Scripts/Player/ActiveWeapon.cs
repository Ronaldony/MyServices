using System.Collections;
using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }

    private WeaponInfo _weaponInfo;
    private PlayerControls playerControls;
    private float timeBetweenAttacks;

    private bool attackButtonDown, isAttacking = false;

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();

        AttackCooldown();
    }

    private void Update()
    {
        Attack();

        if (_weaponInfo != null)
        {
            switch (_weaponInfo.weaponId)
            {
                case 1: // Sword.
                    {
                        // Angle.
                        Vector3 mousePos = Input.mousePosition;
                        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);
                        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

                        // Direction.
                        var isLeft = mousePos.x < playerScreenPoint.x ? true : false;

                        // Sword.
                        var sword = CurrentActiveWeapon.GetComponent<Sword>();
                        sword.FollowAngle(isLeft, angle);
                    }
                    break;
                case 2: // Bow.
                    {
                        // Angle.
                        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector2 direction = mousePoint - transform.position;

                        // Bow.
                        var bow = CurrentActiveWeapon.GetComponent<Bow>();
                        bow.FollowAngle(direction);
                    }
                    break;
            }

        }
    }

    public void NewWeapon(MonoBehaviour newWeapon)
    {
        CurrentActiveWeapon = newWeapon;

        _weaponInfo = (CurrentActiveWeapon as IWeapon).GetWeaponInfo();

        AttackCooldown();
        timeBetweenAttacks = (CurrentActiveWeapon as IWeapon).GetWeaponInfo().weaponCooldown;
    }

    public void WeaponNull()
    {
        CurrentActiveWeapon = null;
    }

    private void AttackCooldown()
    {
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetweenAttacksRoutine());
    }

    private IEnumerator TimeBetweenAttacksRoutine()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }

    public void ToggleIsAttacking(bool value)
    {
        isAttacking = value;
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack()
    {
        // 인벤토리 창이 켜져있는 경우, 무시.
        if (Inventory.Instance.IsShowInventory() || PostBox.Instance.IsShowPostBox())
        {
            return;
        }

        if (attackButtonDown && !isAttacking && CurrentActiveWeapon)
        {
            AttackCooldown();

            (CurrentActiveWeapon as IWeapon).Attack();
        }
    }
}
