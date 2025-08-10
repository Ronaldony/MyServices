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
        MobHealth enemyHealth = collision.gameObject.GetComponent<MobHealth>();
        Indestructible indestructible = collision.gameObject.GetComponent<Indestructible>();
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        // Cond 1. Trigger collision.
        // Cond 2. EnemyHealth Indestructible.
        if (!collision.isTrigger && (enemyHealth || indestructible || playerHealth))
        {
            if ((playerHealth && isEnemyProjectile) || (enemyHealth && !isEnemyProjectile))
            {
                playerHealth?.TakeDamage(1, transform);

                // Collision.
                Instantiate(particleOnPrefabVFX, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else if (indestructible)
            {
                // Collision.
                Instantiate(particleOnPrefabVFX, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    private void DetectFireDistance()
    {
        // Detect fire distance.
        if (Vector3.Distance(transform.position, startPosition) > projectileRange)
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        // Move project tile.
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }
}
