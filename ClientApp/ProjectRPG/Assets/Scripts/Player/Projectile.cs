using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particleOnPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 10f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    public void UpdateMoveSpeed(float speed)
    {
        this.moveSpeed = speed;
    }

    public void UpdateProjectileRange(float projectileRange)
    {
        this.projectileRange = projectileRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        Indestructible indestructible = collision.gameObject.GetComponent<Indestructible>();
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        // Cond 1. Trigger 속성이 아닌 오브젝트.
        // Cond 2. EnemyHealth나 Indestructible 오브젝트인 경우.
        if (!collision.isTrigger && (enemyHealth || indestructible || playerHealth))
        {
            if ((playerHealth && isEnemyProjectile) || (enemyHealth && !isEnemyProjectile))
            {
                playerHealth?.TakeDamage(1, transform);

                // 파괴 파티클 생성.
                Instantiate(particleOnPrefabVFX, transform.position, transform.rotation);

                // 투사체 파괴.
                Destroy(gameObject);
            }
            else if (!collision.isTrigger && indestructible)
            {
                // 파괴 파티클 생성.
                Instantiate(particleOnPrefabVFX, transform.position, transform.rotation);

                // 투사체 파괴.
                Destroy(gameObject);
            }
        }
    }

    private void DetectFireDistance()
    {
        // 무기 범위 초과 시 투사체 파괴.
        if (Vector3.Distance(transform.position, startPosition) > projectileRange)
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }
}
