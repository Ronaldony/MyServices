using GameCore;
using UnityEngine;

public class Grape : MonoBehaviour, IMob
{
    [SerializeField] private GameObject grapeProjectilePrefab;

    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack(Vector2 targetPos, Vector2 projectileSpawnPos)
    {
        myAnimator.SetTrigger(ATTACK_HASH);

        if (transform.position.x - targetPos.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        // Spawn projectile.
        var grapeObject = Instantiate(grapeProjectilePrefab, projectileSpawnPos, Quaternion.identity);
        var grape = grapeObject.GetComponent<GrapeProjectile>();

        // Set damage.
        var mobAI = GetComponent<MobAI>();
        var damage = mobAI.GetTBMob().Damage;

        // Set damage.
        grape.SetDamage(damage);
    }
}
