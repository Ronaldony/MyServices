
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
    void Attack(Vector2 dir, float rotation);

    WeaponInfo GetWeaponInfo();
}