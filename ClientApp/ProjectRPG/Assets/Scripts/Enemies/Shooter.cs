using GameCore;
using UnityEngine;

public class Shooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int _projectileId;

    private float _speed;

    private void Start()
    {
        var tbProjectile = TDBManagement.Instance.GetById<TB_Projectile>(_projectileId);
        _speed = tbProjectile.Velocity;
    }

    private void OnValidate()
    {

    }

    public void Attack(Vector2 targetPos, Vector2 projectileSpawnPos)
    {
        //if (transform.position.x - targetPos.x < 0)
        //{
        //    spriteRenderer.flipX = false;
        //}
        //else
        //{
        //    spriteRenderer.flipX = true;
        //}

        // Spawn projectile.
        GameObject bullet = Instantiate(bulletPrefab, projectileSpawnPos, Quaternion.identity);

        bullet.transform.right = bullet.transform.position - transform.position;

        if (bullet.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.UpdateMoveSpeed(_speed);
        }
    }
}
