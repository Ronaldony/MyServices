using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private GameObject _deathVFXPrefab;
    [SerializeField] private float _knockBackThrust = 10f;

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
    public void TakeDamage(int damage)
    {
        _health -= damage;

        // 충돌 처리 간 넉백 처리.
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
        // 체력이 0 이하인 경우 죽음 처리.
        if (_health <= 0)
        {
            // 데스 이펙트 생성.
            Instantiate(_deathVFXPrefab, transform.position, Quaternion.identity);

            // 드랍 아이템.
            GetComponent<PickUpSpawner>().DropItems();

            // 적 오브젝트 파괴.
            Destroy(gameObject);

            // EnemyManagement에 죽은 적 전달.
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
