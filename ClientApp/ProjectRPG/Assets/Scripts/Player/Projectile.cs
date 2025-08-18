using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject particleOnPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;

    // Projectiles.
    private int _damage;
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

    public void Setup(float speed, float projectileRange, int damage)
    {
        _damage = damage;
        this._moveSpeed = speed;
        this._range = projectileRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var mobHealth = collision.gameObject.GetComponent<MobHealth>();
        var indestructible = collision.gameObject.GetComponent<Indestructible>();
        var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        // Cond 1. Trigger collision.
        // Cond 2. EnemyHealth Indestructible.
        if (!collision.isTrigger)
        {
            if (playerHealth && isEnemyProjectile)
            {
                playerHealth?.TakeDamage(_damage, transform);

                // Collision.
                Instantiate(particleOnPrefabVFX, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else if (mobHealth && !isEnemyProjectile)
            {
                mobHealth?.TakeDamage(_damage, transform.position);

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
