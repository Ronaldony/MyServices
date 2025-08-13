using GameCore;
using UnityEngine;

public class Shooter : MonoBehaviour, IMob
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int _projectileId;

    private TB_Projectile _tbProjectile;
    private float _range;

    private void Start()
    {
        _tbProjectile = TDBManagement.Instance.GetById<TB_Projectile>(_projectileId);
    }

    private void OnValidate()
    {

    }

    public void Attack(Vector2 targetPos, Vector2 projectileSpawnPos)
    {
        // Spawn projectile.
        GameObject bullet = Instantiate(bulletPrefab, projectileSpawnPos, Quaternion.identity);

        bullet.transform.right = new Vector3(targetPos.x, targetPos.y)  - bullet.transform.position;

        if (bullet.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.UpdateMoveSpeed(_tbProjectile.Velocity);
            projectile.UpdateProjectileRange(_tbProjectile.MaxRange);
        }
    }
}
