using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private void Start()
    {
        MonoBehaviour activeWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemyHealth = other.gameObject.GetComponent<MobHealth>();
        enemyHealth?.TakeDamage();
    }
}
