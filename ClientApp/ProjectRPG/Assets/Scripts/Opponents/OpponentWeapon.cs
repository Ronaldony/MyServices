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
    public void Attack(Vector2 angle, float rotation)
    {
        if (null != CurrentActiveWeapon)
        {
            var controller = GetComponentInParent<OpponentController>();
            controller.AdjustPlayerFacingDirection(angle.x);

            (CurrentActiveWeapon as IWeapon).Attack(angle, rotation);
        }
    }

    /// <summary>
    /// Attack.
    /// </summary>
    public void Attack(Vector2 spawn, Vector2 target)
    {
        if (null != CurrentActiveWeapon)
        {
            // Angle.
            //var rotation = Quaternion.FromToRotation(new Vector3(spawn.X, spawn.Y), new Vector3(target.X, target.Y));

            (CurrentActiveWeapon as IWeapon).Attack((target - spawn).normalized, float.MinValue);
        }
    }

}
