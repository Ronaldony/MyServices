using GameCore;
using Network;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

    // Status.
    private int _enemyId;
    private bool _canAttack = true;

    // Roaming.
    private List<Vector2> _roamingPos;
    private Vector2 _nextPos;

    // Path.
    private State _state;
    private EnemyPathfinding _enemyPathfinding;

    // Update Packet.
    private ConcurrentQueue<UpdatePacket> _updatePacketQ;

    private void Awake()
    {
        _state = State.Roaming;
        _enemyPathfinding = GetComponent<EnemyPathfinding>();
        _updatePacketQ = new ConcurrentQueue<UpdatePacket>();
    }

    private void Start()
    {
        //_nextPos = GetRoamingPosition();
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
    public void Initialize_Spawn(
        int enemyId,
        int attackRange, 
        int attackCooldown, 
        System.Numerics.Vector2 nextPos,
        List<System.Numerics.Vector2> roamingPositions)
    {
        _enemyId = enemyId;
        _attackRange = attackRange;
        _attackCooldown = attackCooldown;

        // Convert roaming.
        _roamingPos = new List<Vector2>();
        foreach (var pos in roamingPositions)
        {
            _roamingPos.Add(new Vector2(pos.X, pos.Y));
        }

        // Set next position.
        _nextPos = new Vector2(nextPos.X, nextPos.Y);
    }

    /// <summary>
    /// Roaming.
    /// </summary>
    private void Roaming()
    {
        // 플레이어가 공격 사정거리 안에 있는 경우, 공격 상태로 전환.
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < _attackRange)
        {
            _state = State.Attacking;
        }

        // Update packet.
        if (_updatePacketQ.TryDequeue(out var updatePacket))
        {
            switch (updatePacket.PacketCode)
            {
                case NetPacketCode.SC_MoveStart:
                    {
                        // TODO: Set next position. 
                        // _nextPos = updatePacket.NextPos;
                        _enemyPathfinding.MoveTo(_nextPos);
                    }
                    break;
            }
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
    /// Get enemy id.
    /// </summary>
    public int GetEnemyId() => _enemyId;

    /// <summary>
    /// Enqueue update packet.
    /// </summary>
    public void EnqueueUpdatePacket(UpdatePacket updatePacket)
    {
        _updatePacketQ.Enqueue(updatePacket);
    }
}
