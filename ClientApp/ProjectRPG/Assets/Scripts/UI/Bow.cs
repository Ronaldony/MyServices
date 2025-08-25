using GameCore;
using GameCore.Datas.Projectiles;
using GameCore.GamePacket;
using Network;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    readonly int FIRE_HASH = Animator.StringToHash("Fire");

    private Animator myAnimator;

    private GamePacketManager _gpm;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        _gpm = GamePacketManager.Instance;
    }

    private void Update()
    {
        MouseFollowWithOffset();
    }

    public void Attack()
    {
        myAnimator.SetTrigger(FIRE_HASH);

        // Arrow.
        GameObject arrowObject = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
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
                SpawnPos = new System.Numerics.Vector2(arrowSpawnPoint.position.x, arrowSpawnPoint.position.y),
                TargetPos = new System.Numerics.Vector2(mousePos.x, mousePos.y)
            }
        });
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = transform.position - playerScreenPoint;

        ActiveWeapon.Instance.transform.right = -direction;
    }
}
