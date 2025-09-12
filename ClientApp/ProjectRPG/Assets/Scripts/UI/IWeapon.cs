
using UnityEngine;

/// <summary>
/// IWeapon.
/// </summary>
interface IWeapon
{
    /// <summary>
    /// Player attack.
    /// </summary>
    void Attack();

    /// <summary>
    /// Opponent attack.
    /// </summary>
    void Attack(Quaternion rotation);

    WeaponInfo GetWeaponInfo();
}