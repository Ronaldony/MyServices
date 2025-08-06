using GameCore.Datas.Projectiles;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Attacking
    }

    [SerializeField] private MonoBehaviour _mobType;

    private long _unitId;

    /// <summary>
    /// Attack.
    /// </summary>
    public void Attack(Vector2 targetPos)
    {
        (_mobType as IEnemy)?.Attack(new Vector2(targetPos.x, targetPos.y), Vector2.zero);
    }

    /// <summary>
    /// Attack.
    /// </summary>
    public void AttackProjectile(DTO_Projectile projectile)
    {
        (_mobType as IEnemy)?.Attack(
            new Vector2(projectile.TargetPos.X, projectile.TargetPos.Y), 
            new Vector2(projectile.SpawnPos.X, projectile.SpawnPos.Y));
    }

    /// <summary>
    /// Get unit id.
    /// </summary>
    public long GetUnitId() => _unitId;
}
