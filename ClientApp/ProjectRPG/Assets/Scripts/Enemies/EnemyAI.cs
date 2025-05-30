using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Attacking
    }

    [SerializeField] private MonoBehaviour _enemyType;

    [SerializeField] private float _attackRange = 0f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private bool _stopMovingWhileAttacking = false;
    [SerializeField] private int _enemyId;

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
        MovementStateControl();
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

            (_enemyType as IEnemy)?.Attack();

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
    /// Get enemy id.
    /// </summary>
    public int GetEnemyId() => _enemyId;
}
