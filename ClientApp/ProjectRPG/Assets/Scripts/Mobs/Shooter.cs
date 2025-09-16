using GameCore;
using UnityEngine;

public class Shooter : MonoBehaviour, IMob
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int _projectileId;

    public void Attack(Vector2 targetPos, Vector2 projectileSpawnPos)
    {
        // Spawn projectile.
        GameObject bullet = Instantiate(bulletPrefab, projectileSpawnPos, Quaternion.identity);

        bullet.transform.right = new Vector3(targetPos.x, targetPos.y)  - bullet.transform.position;

        if (bullet.TryGetComponent<Projectile>(out var projectile))
        {
            var mobAI = GetComponent<MobAI>();
            var tbMob = mobAI.GetTBMob();

            var tbProjectile = TDBManagement.Instance.GetById<TB_Projectile>(_projectileId);

            projectile?.Setup(tbProjectile.Velocity, tbProjectile.MaxRange);
        }
    }
}
