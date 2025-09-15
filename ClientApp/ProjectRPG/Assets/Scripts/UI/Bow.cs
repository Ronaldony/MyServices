using GameCore;
using GameCore.Datas.Projectiles;
using GameCore.GamePacket;
using GamePackets;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    readonly int FIRE_HASH = Animator.StringToHash("Fire");

    private Animator myAnimator;

    private GamePacketManager _gpm;

    // Controller.
    private PlayerController _playerController;
    private OpponentController _opponentController;

    private Transform _activeWeaponTrans;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        _gpm = GamePacketManager.Instance;

        // Player.
        var player = GetComponentInParent<PlayerController>();

        if (null != player)
        {
            _playerController = player;
            _activeWeaponTrans = ActiveWeapon.Instance.transform;
        }

        // Opponent.
        var opponent = GetComponentInParent<OpponentController>();

        if (null != opponent)
        {
            _opponentController = opponent;
            _activeWeaponTrans = GetComponentInParent<OpponentWeapon>().transform;
        }
    }

    /// <summary>
    /// Player Attack
    /// </summary>
    public void Attack()
    {
        myAnimator.SetTrigger(FIRE_HASH);

        // Arrow.
        GameObject arrowObject = Instantiate(arrowPrefab, transform.position, ActiveWeapon.Instance.transform.rotation);
        Projectile projectile = arrowObject.GetComponent<Projectile>();

        var tbWeapon = TDBManagement.Instance.GetById<TB_Weapon>(weaponInfo.weaponId);
        var tbProjectile = TDBManagement.Instance.First<TB_Projectile>(d => d.ProjectileType == tbWeapon.ProjectileType);

        projectile.Setup(tbProjectile.Velocity, tbProjectile.MaxRange);

        // Mouse pos.
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        _gpm.Send<GamePacketCS_AttackProjectile>(GamePacketCode.CS_AttackProjectile, new GamePacketCS_AttackProjectile
        {
            Projectile = new DTO_Projectile
            {
                ProjectileType = Type_Projectile.Arrow,
                SpawnPos = new System.Numerics.Vector2(transform.position.x, transform.position.y),
                TargetPos = new System.Numerics.Vector2(mousePos.x, mousePos.y)
            }
        });
    }

    /// <summary>
    /// Opponent attack.
    /// </summary>
    public void Attack(Vector2 dir, float rotation)
    {
        FollowAngle(dir);

        myAnimator.SetTrigger(FIRE_HASH);

        // Arrow.
        GameObject arrowObject = Instantiate(arrowPrefab, transform.position, _activeWeaponTrans.rotation);
        Projectile projectile = arrowObject.GetComponent<Projectile>();

        var tbWeapon = TDBManagement.Instance.GetById<TB_Weapon>(weaponInfo.weaponId);
        var tbProjectile = TDBManagement.Instance.First<TB_Projectile>(d => d.ProjectileType == tbWeapon.ProjectileType);

        projectile.Setup(tbProjectile.Velocity, tbProjectile.MaxRange);
    }
    
    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void FollowAngle(Vector2 direction)
    {
        _activeWeaponTrans.right = direction;
    }
}
