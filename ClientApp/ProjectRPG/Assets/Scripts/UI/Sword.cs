using GameCore;
using GameCore.GamePacket;
using GamePackets;
using System.Linq;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject slashAnimPrefab;
    private Transform _slashAnimSpawnPos;

    private Transform weaponCollider;
    private Animator myAnimator;

    private GamePacketManager _gpm;

    private GameObject slashAnim;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();

        _gpm = GamePacketManager.Instance;
    }

    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();

        var activeWeapon = GetComponentsInParent<Transform>().FirstOrDefault(d => d.name == "Active Weapon");
        _slashAnimSpawnPos = activeWeapon.GetComponentsInChildren<Transform>().FirstOrDefault(d => d.name == "SlashSpawnPoint");
    }

    private void Update()
    {
        MouseFollowWithOffeset();
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void Attack()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);
        Vector2 angle = (mousePos - playerScreenPoint).normalized;

        // Send Attack.
        _gpm.Send<GamePacketCS_AttackMelee>(
            GamePacketCode.CS_AttackMelee,
            new GamePacketCS_AttackMelee
            {
                Angle = new System.Numerics.Vector2(angle.x, angle.y),
            });

        // set animation.
        myAnimator.SetTrigger("Attack");

        // Visible weapon collider.
        weaponCollider.gameObject.SetActive(true);
        weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = true;

        // Slash prefab.
        slashAnim = Instantiate(slashAnimPrefab, _slashAnimSpawnPos.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;
    }

    /// <summary>
    /// Opponent attack.
    /// </summary>
    public void Attack(Quaternion rotation)
    {
        // set animation.
        myAnimator.SetTrigger("Attack");

        // Visible weapon collider.
        weaponCollider.gameObject.SetActive(true);
        weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = true;

        // Slash prefab.
        slashAnim = Instantiate(slashAnimPrefab, _slashAnimSpawnPos.position, rotation);
        slashAnim.transform.parent = this.transform.parent;
    }

    public void DoneAttackingAnimEvent()
    {
        weaponCollider.gameObject.SetActive(false);
    }

    public void SwingUpFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);
        
        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void MouseFollowWithOffeset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
