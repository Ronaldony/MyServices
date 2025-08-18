using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject particleOnPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;

    // Projectiles.
    private float _moveSpeed;
    private float _range;

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
        this._moveSpeed = speed;
    }

    public void UpdateProjectileRange(float projectileRange)
    {
        this._range = projectileRange;
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
            if (playerHealth && isEnemyProjectile)
            {
                playerHealth?.TakeDamage(1, transform);

                // Collision.
                Instantiate(particleOnPrefabVFX, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else if (enemyHealth && !isEnemyProjectile)
            {
                enemyHealth?.TakeDamage(1, transform.position);

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
        if (Vector3.Distance(transform.position, startPosition) > _range)
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        // Move project tile.
        transform.Translate(Vector3.right * Time.deltaTime * _moveSpeed);
    }
}
