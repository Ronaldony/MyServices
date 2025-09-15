using GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpponentWeaponInventory : MonoBehaviour
{
    [SerializeField] private List<WeaponInfo> _weaponInfos;

    /// <summary>
    /// Change active weapon.
    /// </summary>
    public void ChangeActiveWeapon(int weaponId)
    {
        var weaponComp = GetComponentInChildren<OpponentWeapon>();

        // Destroy.
        if (weaponComp.CurrentActiveWeapon != null)
        {
            Destroy(weaponComp.CurrentActiveWeapon.gameObject);
        }

        WeaponInfo weaponInfo = _weaponInfos.FirstOrDefault(d => d.weaponId == weaponId);

        if (null == weaponInfo)
        {
            Debug.LogError($"Can't find weapon. id: {weaponId}");
            weaponComp.WeaponNull();
            return;
        }

        // TB_Weapon.
        var tbWeapon = TDBManagement.Instance.GetById<TB_Weapon>(weaponId);
        if (null == tbWeapon)
        {
            Debug.LogError("No weapon!");
            weaponComp.WeaponNull();
            return;
        }

        // Refresh WeaponInfo.
        weaponInfo.weaponId = tbWeapon.Id;

        // New weapon.
        GameObject weaponToSpawn = weaponInfo.weaponPrefab;
        var newWeapon = Instantiate(weaponToSpawn, weaponComp.transform);

        weaponComp.SetNewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }

    /// <summary>
    /// Attack melee.
    /// </summary>
    public void AttackMelee(System.Numerics.Vector2 angle, float rotation)
    {
        var weaponComp = GetComponentInChildren<OpponentWeapon>();

        weaponComp?.Attack(new Vector2(angle.X, angle.Y), rotation);
    }

    /// <summary>
    /// Attack projectile.
    /// </summary>
    public void AttackProjectile(System.Numerics.Vector2 spawn, System.Numerics.Vector2 target)
    {
        var weaponComp = GetComponentInChildren<OpponentWeapon>();

        weaponComp?.Attack(new Vector2(spawn.X, spawn.Y), new Vector2(target.X, target.Y));
    }
}
