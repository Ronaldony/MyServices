using UnityEngine;

public class MobHealth : MonoBehaviour
{
    [SerializeField] private GameObject _deathVFXPrefab;

    private int _health;
    private Knockback _knockback;
    private Flash _flash;
    private long _unitId;

    private void Awake()
    {
        _flash = GetComponent<Flash>();
        _knockback = GetComponent<Knockback>();
    }

    /// <summary>
    /// Take damage.
    /// </summary>
    public void TakeDamage(Vector3 srcPosition)
    {
        // Knockback.
        //_knockback.GetKnockedBack(srcPosition, GameData.KNOCKBACK_THRUST);

        // hit routine.
        StartCoroutine(_flash.FlashRoutine());
    }

    /// <summary>
    /// Set health.
    /// </summary>
    public void UpdateHealth(int hp)
    {
        _health = hp;

        DetectDeath();
    }

    /// <summary>
    /// Take damage.
    /// </summary>
    public void TakeDamage()
    {
        // hit routine.
        StartCoroutine(_flash.FlashRoutine());
    }

    /// <summary>
    /// Detect death
    /// </summary>
    public void DetectDeath()
    {
        // Detect death.
        if (_health <= 0)
        {
            // Create VFX for death.
            Instantiate(_deathVFXPrefab, transform.position, Quaternion.identity);

            // Drop item.
            //GetComponent<PickUpSpawner>().DropItems();

            // Destroy.
            Destroy(gameObject);

            // Kill enemy.
            EnemyManagement.Instance.KillEnemy(_unitId);
        }
    }

    /// <summary>
    /// Initialize spawn.
    /// </summary>
    public void SetupHealth(int health, long unitId)
    {
        _health = health;
        _unitId = unitId;
    }
}
