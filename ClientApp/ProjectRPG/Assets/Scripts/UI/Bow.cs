using GameCore;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    readonly int FIRE_HASH = Animator.StringToHash("Fire");

    private Animator myAnimator;

    private TB_Weapon _tbWeapon;
    private TB_Projectile _tbProjectile;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();

        _tbWeapon = TDBManagement.Instance.GetById<TB_Weapon>(weaponInfo.weaponId);
        _tbProjectile = TDBManagement.Instance.First<TB_Projectile>(d => d.ProjectileType == _tbWeapon.ProjectileType);
    }

    public void Attack()
    {
        myAnimator.SetTrigger(FIRE_HASH);

        // Arrow.
        GameObject arrowObject = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
        Projectile projectile = arrowObject.GetComponent<Projectile>();

        projectile.Setup(_tbProjectile.Velocity, _tbWeapon.Range, _tbWeapon.Damage);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}
