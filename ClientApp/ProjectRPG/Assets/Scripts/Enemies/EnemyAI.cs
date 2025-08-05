using GameCore.Datas.Projectiles;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Attacking
    }

    [SerializeField] private MonoBehaviour _mobType;

    [SerializeField] private float _attackRange = 0f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private bool _stopMovingWhileAttacking = false;
    [SerializeField] private int _unitId;

    // Status.
    private bool _canAttack = true;

    // Path.
    private State _state;
    private EnemyPathfinding _enemyPathfinding;

    private void Awake()
    {
        _state = State.Roaming;
        _enemyPathfinding = GetComponent<EnemyPathfinding>();
    }

    private void Update()
    {
        //MovementStateControl();

    }

    /// <summary>
    /// Update movement state,
    /// </summary>
    private void MovementStateControl()
    {
        switch (_state)
        {
            default:
            case State.Roaming:
                Roaming();
                break;
            case State.Attacking:
                Attacking();
                break;
        }
    }

    /// <summary>
    /// Initailize spawn.
    /// </summary>
    public void Initialize_Spawn(int attackRange, int attackCooldown)
    {
        _attackRange = attackRange;
        _attackCooldown = attackCooldown;
    }

    /// <summary>
    /// Roaming.
    /// </summary>
    private void Roaming()
    {
        // Calculate distance for attacking.
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < _attackRange)
        {
            _state = State.Attacking;
        }
    }

    /// <summary>
    /// Attack.
    /// </summary>
    private void Attack()
    {
        (_mobType as IEnemy)?.Attack();
    }

    /// <summary>
    /// Attack.
    /// </summary>
    private void AttackProjectile(DTO_Projectile projectile)
    {
        (_mobType as IEnemy)?.Attack();
    }

    /// <summary>
    /// Try to attack.
    /// </summary>
    private void Attacking()
    {
        // Calculate distance for attacking.
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > _attackRange)
        {
            _state = State.Roaming;
        }

        if (_attackRange != 0 && _canAttack)
        {
            _canAttack = false;

            (_mobType as IEnemy)?.Attack();

            if (_stopMovingWhileAttacking)
            {
                _enemyPathfinding.StopMoving();
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    /// <summary>
    /// Attack coll down routine.
    /// </summary>
    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }

    /// <summary>
    /// Get unit id.
    /// </summary>
    public int GetUnitId() => _unitId;
}
