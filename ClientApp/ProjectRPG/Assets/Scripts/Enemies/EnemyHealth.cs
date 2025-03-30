using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private GameObject _deathVFXPrefab;
    [SerializeField] private float _knockBackThrust = 10f;

    private int _health;
    private Knockback _knockback;
    private Flash _flash;

    private void Awake()
    {
        _flash = GetComponent<Flash>();
        _knockback = GetComponent<Knockback>();
    }

    private void Start()
    {

    }

    /// <summary>
    /// Take damage.
    /// </summary>
    public void TakeDamage(int damage)
    {
        _health -= damage;

        // 面倒 贸府 埃 乘归 贸府.
        _knockback.GetKnockedBack(PlayerController.Instance.transform, _knockBackThrust);
        StartCoroutine(_flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    /// <summary>
    /// Check detect death routine.
    /// </summary>
    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(_flash.GetRestoreMatTime());
        DetectDeath();
    }

    /// <summary>
    /// Detect death
    /// </summary>
    public void DetectDeath()
    {
        if (_health <= 0)
        {
            Instantiate(_deathVFXPrefab, transform.position, Quaternion.identity);
            GetComponent<PickUpSpawner>().DropItems();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initialize spawn.
    /// </summary>
    public void Initialize_Spawn(int health)
    {
        _health = health;
    }
}
