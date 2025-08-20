using UnityEngine;
using GameCore.Datas.Projectiles;
using GameCore;

public class MobAI : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _mobType;

    [SerializeField] private int _mobId;

    private TB_Mob _tbMob;

    private void Start()
    {
        _tbMob = TDBManagement.Instance.GetById<TB_Mob>(_mobId);
    }

    public TB_Mob GetTBMob()
    {
        return _tbMob;
    }

    /// <summary>
    /// Attack.
    /// </summary>
    public void Attack(Vector2 targetPos)
    {
        (_mobType as IMob)?.Attack(new Vector2(targetPos.x, targetPos.y), Vector2.zero);
    }

    /// <summary>
    /// Attack.
    /// </summary>
    public void AttackProjectile(DTO_Projectile projectile)
    {
        (_mobType as IMob)?.Attack(
            new Vector2(projectile.TargetPos.X, projectile.TargetPos.Y), 
            new Vector2(projectile.SpawnPos.X, projectile.SpawnPos.Y));
    }

    /// <summary>
    /// Get unit id.
    /// </summary>
    public int GetMobId() => _mobId;
}
