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

    private Transform _weaponCollider;
    private Animator _myAnimator;

    private GamePacketManager _gpm;

    private GameObject _slashAnim;

    // Controller.
    private PlayerController _playerController;
    private OpponentController _opponentController;

    private Transform _activeWeaponTrans;

    private void Awake()
    {
        _myAnimator = GetComponent<Animator>();

        _gpm = GamePacketManager.Instance;
    }

    private void Start()
    {
        // Player.
        var player = GetComponentInParent<PlayerController>();

        if (null != player)
        {
            _playerController = player;
            _weaponCollider = PlayerController.Instance.GetWeaponCollider();
            _activeWeaponTrans = ActiveWeapon.Instance.transform;
        }

        // Opponent.
        var opponent = GetComponentInParent<OpponentController>();

        if (null != opponent)
        {
            _opponentController = opponent;
            _weaponCollider = opponent.GetWeaponCollider();
            _activeWeaponTrans = GetComponentInParent<OpponentWeapon>().transform;
        }

        var activeWeapon = GetComponentsInParent<Transform>().FirstOrDefault(d => d.name == "Active Weapon");
        _slashAnimSpawnPos = activeWeapon.GetComponentsInChildren<Transform>().FirstOrDefault(d => d.name == "SlashSpawnPoint");
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void Attack()
    {
        var mousePos = Input.mousePosition;
        var playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);
        Vector2 angle = (mousePos - playerScreenPoint).normalized;
        var rotation = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        // Send Attack.
        _gpm.Send<GamePacketCS_AttackMelee>(
            GamePacketCode.CS_AttackMelee,
            new GamePacketCS_AttackMelee
            {
                Angle = new System.Numerics.Vector2(angle.x, angle.y),
                Rotation = rotation
            });

        // set animation.
        _myAnimator.SetTrigger("Attack");

        // Visible weapon collider.
        _weaponCollider.gameObject.SetActive(true);
        _weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        _weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = true;

        // Slash prefab.
        _slashAnim = Instantiate(slashAnimPrefab, _slashAnimSpawnPos.position, Quaternion.identity);
        _slashAnim.transform.parent = this.transform.parent;
    }

    /// <summary>
    /// Opponent attack.
    /// </summary>
    public void Attack(Vector2 dir, float rotation)
    {
        var isLeft = dir.x < 0 ? true : false;

        // Mouse follow.
        FollowAngle(isLeft, rotation);

        _myAnimator.SetTrigger("Attack");

        // Visible weapon collider.
        _weaponCollider.gameObject.SetActive(true);
        _weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        _weaponCollider.gameObject.GetComponent<PolygonCollider2D>().enabled = true;

        // Slash prefab.
        _slashAnim = Instantiate(slashAnimPrefab, _slashAnimSpawnPos.position, Quaternion.identity);
        _slashAnim.transform.parent = this.transform.parent;
    }

    public void DoneAttackingAnimEvent()
    {
        _weaponCollider.gameObject.SetActive(false);
    }

    public void SwingUpFlipAnimEvent()
    {
        _slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        var isLeft = IsLeft();

        if (isLeft)
        {
            _slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        _slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        var isLeft = IsLeft();

        if (isLeft)
        {
            _slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    /// <summary>
    /// Check left.
    /// </summary>
    private bool IsLeft()
    {
        bool isLeft = false;

        if (_playerController)
        {
            isLeft = PlayerController.Instance.FacingLeft;
        }
        else
        {
            isLeft = _opponentController.FacingLeft;
        }

        return isLeft;
    }

    /// <summary>
    /// Follow angle.
    /// </summary>
    public void FollowAngle(bool isLeft, float angle)
    {
        if (isLeft)
        {
            // 왼쪽.
            if (_activeWeaponTrans) _activeWeaponTrans.rotation =  Quaternion.Euler(0, -180, angle);
            if (_weaponCollider) _weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            // 오른쪽.
            if (_activeWeaponTrans) _activeWeaponTrans.rotation = Quaternion.Euler(0, 0, angle);
            if (_weaponCollider) _weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
