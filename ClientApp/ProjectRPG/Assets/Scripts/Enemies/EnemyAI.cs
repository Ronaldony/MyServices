using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Attacking
    }

    [SerializeField] private int _enemyId;
    [SerializeField] private MonoBehaviour _enemyType;

    [SerializeField] private float _attackRange = 0f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private bool _stopMovingWhileAttacking = false;

    // Status.
    private bool _canAttack = true;

    // Roaming.
    private float _roamChangeTime = 5f;
    private List<Vector2> _roamingPos;
    private Vector2 _movePos;
    private float _timeRoaming = 0f;

    // Path.
    private State _state;
    private EnemyPathfinding _enemyPathfinding;

    private void Awake()
    {
        _enemyPathfinding = GetComponent<EnemyPathfinding>();
        _state = State.Roaming;
    }

    private void Start()
    {
        _movePos = GetRoamingPosition();
    }

    private void Update()
    {
        MovementStateControl();
    }

    /// <summary>
    /// Initailize spawn.
    /// </summary>
    public void Initialize_Spawn(int attackRange, int attackCooldown, List<Vector2> roaminPosList)
    {
        _attackRange = attackRange;
        _attackCooldown = attackCooldown;
        _roamingPos = roaminPosList;
        _movePos = roaminPosList.First();
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
    /// Roaming.
    /// </summary>
    private void Roaming()
    {
        _timeRoaming += Time.deltaTime;

        // 플레이어가 공격 사정거리 안에 있는 경우, 공격 상태로 전환.
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < _attackRange)
        {
            _state = State.Attacking;
        }
        
        if (_timeRoaming > _roamChangeTime)
        {
            _enemyPathfinding.MoveTo(_movePos, _roamChangeTime);
            _movePos = GetRoamingPosition();
        }
    }

    /// <summary>
    /// Try to attack.
    /// </summary>
    private void Attacking()
    {
        // 플레이어가 공격 사정거리 밖인 경우, 로밍 상태로 전환.
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
            else
            {
                //_enemyPathfinding.MoveTo(_movePos, _movePos.normalized, _roamChangeDir);
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
    /// Get romaing position.
    /// </summary>
    private Vector2 GetRoamingPosition()
    {
        _timeRoaming = 0f;

        var index = _roamingPos.IndexOf(_movePos);

        return _roamingPos.ElementAt((index + 1) % _roamingPos.Count);
    }

    /// <summary>
    /// Get enemy id.
    /// <returns></returns>
    public int GetEnemyId()
    {
        return _enemyId;
    }
}
