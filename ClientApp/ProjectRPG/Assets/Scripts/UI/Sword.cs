using GameCore;
using GameCore.NetPacket;
using Network;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;

    private Transform weaponCollider;
    private Animator myAnimator;

    private NetPacketManager _netPacketManager;
    private PlayerDataManager _playerDataManager;

    private GameObject slashAnim;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();

        _netPacketManager = NetPacketManager.Instance;
        _playerDataManager = PlayerDataManager.Instance;
    }

    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = GameObject.Find("SlashSpawnPoint").transform;
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

        Debug.Log($"Attacl angle: {angle}");
        
        // Send Attack.
        _netPacketManager.Send<NetPacketCS_Attack>(
            NetPacketCode.CS_Attack,
            new NetPacketCS_Attack
            {
                Pid = _playerDataManager.Pid,
                Angle = new System.Numerics.Vector2(angle.x, angle.y),
                Damage = 3
            });

        // set animation.
        myAnimator.SetTrigger("Attack");

        // Visible weapon collider.
        weaponCollider.gameObject.SetActive(true);
        weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = true;

        // Slash prefab.
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
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
