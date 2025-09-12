using System;
using UnityEngine;

/// <summary>
/// OpponentWeapon.
/// </summary>
public class OpponentWeapon : MonoBehaviour
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }

    public void SetNewWeapon(MonoBehaviour newWeapon)
    {
        CurrentActiveWeapon = newWeapon;
    }

    public void WeaponNull()
    {
        CurrentActiveWeapon = null;
    }

    /// <summary>
    /// Attack.
    /// </summary>
    public void Attack(System.Numerics.Vector2 angle)
    {
        if (null != CurrentActiveWeapon)
        {
            var angleDeg = Mathf.Atan2(angle.Y, angle.X) * Mathf.Rad2Deg;
            var rotation = Quaternion.Euler(0, 0, angleDeg);

            (CurrentActiveWeapon as IWeapon).Attack(rotation);
        }
    }

    /// <summary>
    /// Attack.
    /// </summary>
    public void Attack(System.Numerics.Vector2 spawn, System.Numerics.Vector2 target)
    {
        if (null != CurrentActiveWeapon)
        {
            // Angle.
            var rotation = Quaternion.FromToRotation(new Vector3(spawn.X, spawn.Y), new Vector3(target.X, target.Y));

            (CurrentActiveWeapon as IWeapon).Attack(rotation);
        }
    }

}
