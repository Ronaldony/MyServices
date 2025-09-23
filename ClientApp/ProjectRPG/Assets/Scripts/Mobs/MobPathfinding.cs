using GameCore;
using GameCore.Datas.Plays;
using System.Collections.Concurrent;
using UnityEngine;

public class MobPathfinding : MonoBehaviour
{
    private Rigidbody2D _rb;

    private float _velocity;
    private const float MovePerFrame = 0.1f;

    private float _dummyTimePassed;
    private Vector2 _dummyPos = new Vector2();

    private Vector2 _moveDirection = new Vector2();
    private Vector2 _spawnPos;

    private UnitDummy _dummy;
    private Knockback _knockBack;
    private SpriteRenderer _spriteRenderer;

    private readonly ConcurrentQueue<MobMsg> _mobMsgs = new ConcurrentQueue<MobMsg>();

    private void Awake()
    { 
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _knockBack = GetComponent<Knockback>();
        _rb = GetComponent<Rigidbody2D>();

        var mobAI = GetComponent<MobAI>();
        var tbMob = TDBManagement.Instance.GetById<TB_Mob>(mobAI.GetMobId());

        _velocity = tbMob.Speed;
    }

    private void FixedUpdate()
    {
        Proc_Msg();

        MoveToNext();
        FollowDummy();

        if (_moveDirection.x < 0) { _spriteRenderer.flipX = true; }
        else if (_moveDirection.x > 0) { _spriteRenderer.flipX = false; }
    }

    public void AddMobMsg(MobMsg msg)
    {
        _mobMsgs.Enqueue(msg);
    }

    /// <summary>
    /// Setup.
    /// </summary>
    public void Setup(DTO_Location location, UnitDummy dummy = null)
    {
        _spawnPos = new Vector2(location.Pos.X, location.Pos.Y);
        _moveDirection = new Vector2(location.MoveDirection.X, location.MoveDirection.Y);
        transform.position = _spawnPos;

        if (dummy != null)
        {
            _dummy = dummy;
        }

        _dummy?.FixPosition(_spawnPos);
    }

    private void Proc_Msg()
    {
        if (_mobMsgs.TryDequeue(out var msg))
        {
            switch (msg.MsgType)
            {
                case Type_MobMsg.Spawn:
                    var spawnMsg = msg as MobMsg_Spawn;
                    _dummy = spawnMsg.Dummy;

                    var pos = spawnMsg.Location.Pos;
                    _spawnPos = new Vector2(pos.X, pos.Y);
                    break;
                case Type_MobMsg.UpdateMovement:
                    var updateMovementMsg = msg as MobMsg_UpdateMovement;
                    var location = updateMovementMsg.Location;

                    _moveDirection = new Vector2(location.MoveDirection.X, location.MoveDirection.Y);
                    _dummy.FixPosition(new Vector2(location.Pos.X, location.Pos.Y));
                    break;
                case Type_MobMsg.FixMovement:
                    var fixMovementMsg = msg as MobMsg_FixMovement;
                    _dummy.FixPosition(fixMovementMsg.FixPos);
                    break;
            }
        }
    }

    /// <summary>
    /// Check move end.
    /// </summary>
    private bool IsMoveEnd()
    {
        // 조건 1. 이동 가능 범위 검사.
        if (Vector2.Distance(_rb.position, _spawnPos) > GameData.MOB_RETURN_DISTANCE + 4)
        {
            StopMoving();
            //_dummy.FixPosition(_spawnPos);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Stop moving.
    /// </summary>
    public void StopMoving()
    {
        _moveDirection = Vector3.zero;
    }

    /// <summary>
    /// Get position.
    /// </summary>
    public Vector2 GetPosition() => transform.position;

    /// <summary>
    /// Get dummy.
    /// </summary>
    public UnitDummy GetDummy() => _dummy;

    /// <summary>
    /// Move to next.
    /// </summary>
    private void MoveToNext()
    {
        // check next position.
        //_rb.position += _moveDirection * _velocity * Time.fixedDeltaTime;

        if (_moveDirection != Vector2.zero)
        {
            if (IsMoveEnd())
            {
                StopMoving();
            }
            else
            {
                // Move dummy.
                _dummy.MoveTo(_moveDirection * (_velocity * Time.fixedDeltaTime));
            }
        }
    }

    /// <summary>
    /// Follow dummy.
    /// </summary>
    private void FollowDummy()
    {
        // Follow to dummy.
        var newDummyPos = _dummy.GetPosition();

        if (_dummyPos != newDummyPos)
        {
            _dummyPos = newDummyPos;
            _dummyTimePassed = 0;
        }

        _dummyTimePassed += Time.deltaTime;
        float linearT = _dummyTimePassed / MovePerFrame;

        // Move position.
        if (linearT <= 1)
        {
            _rb.MovePosition(Vector2.Lerp(_rb.position, _dummyPos, linearT));
        }
    }
}
