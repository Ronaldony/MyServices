using GameCore;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private GameObject _deathVFXPrefab;

    private int _health;
    private Knockback _knockback;
    private Flash _flash;
    private ulong _enemyUid;

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
    public void TakeDamage(int damage, Vector3 srcPosition)
    {
        Debug.Log($"DetectDeath. health: {_health}");

        //_health -= damage;

        // Knockback.
        //_knockback.GetKnockedBack(srcPosition, GameData.KNOCKBACK_THRUST);

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
        // Detect death.
        if (_health <= 0)
        {
            // ���� ����Ʈ ����.
            Instantiate(_deathVFXPrefab, transform.position, Quaternion.identity);

            // ��� ������.
            GetComponent<PickUpSpawner>().DropItems();

            // �� ������Ʈ �ı�.
            Destroy(gameObject);

            // EnemyManagement�� ���� �� ����.
            EnemyManagement.Instance.KillEnemy(_enemyUid);
        }
    }

    /// <summary>
    /// Initialize spawn.
    /// </summary>
    public void Initialize_Spawn(int health, ulong enemyUid)
    {
        _health = health;
        _enemyUid = enemyUid;
    }
}
