using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private int damageAmount;

    private void Start()
    {
        MonoBehaviour activeWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
        damageAmount = (activeWeapon as IWeapon).GetWeaponInfo().weaponDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DamageSourfce OnTriggerEnter2D.");

        var enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        enemyHealth?.TakeDamage(damageAmount, PlayerController.Instance.transform.position);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"DamageSourfce OnTriggerStay2D.");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"DamageSourfce OnTriggerExit2D.");

    }
}
