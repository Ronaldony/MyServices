
//#define POS_SYNC_1
#define POS_SYNC_2

using GameCore;
using GameCore.Datas.Plays;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

public class OpponentController : MonoBehaviour
{
    public bool FacingLeft { get; set; }

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;
    private const float MovePerFrame = 0.1f;

    private float _dummyTimePassed;
    private Vector2 _dummyPos = new Vector2();
    private Vector2 _spawnPos;
    private Vector2 _refreshMovement;

    private UnitDummy _dummy;
    private Rigidbody2D _rb;
    private Animator _myAnimator;
    private SpriteRenderer _mySpriteRenderer;
    private Knockback _knockback;
    private Flash _flash;
    private OpponentHealth _health;

    private ConcurrentQueue<OpponentMsg> _msgQ = new ConcurrentQueue<OpponentMsg>();

    private Vector2 _moveDir;
    private bool _isDashing;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _myAnimator = GetComponent<Animator>();
        _mySpriteRenderer = GetComponent<SpriteRenderer>();
        _knockback = GetComponent<Knockback>();
        _flash = GetComponent<Flash>();
        _health = GetComponent<OpponentHealth>();

        _rb.position = _spawnPos;
    }

    private void Update()
    {
        UpdateAnaimator();
    }

    private void FixedUpdate()
    {
        // 넉백 혹은 죽음인 경우 처리 넘김.
        if (_health.IsDead())
        {
            return;
        }

        if (_isDashing)
        {
#if POS_SYNC_1
            _rb.MovePosition(_rb.position + _refreshMovement * (moveSpeed * Time.fixedDeltaTime));
#elif POS_SYNC_2
            _rb.MovePosition(_dummy.GetPosition());
#endif
            return;
        }

        // Process msg.
        Proc_Msg();

        // Adjust face 방향.
        AdjustPlayerFacingDirection(_refreshMovement.x);

        // 이전 방향 값 갱신.
        _moveDir = _refreshMovement;

        // Move dummy.
        MoveDummy();

#if POS_SYNC_2
        // Follow dummy.
        FollowDummy();
#endif
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
            _rb.MovePosition(Vector2.Lerp(transform.position, _dummyPos, linearT));
        }
    }

    /// <summary>
    /// Add msg.
    /// </summary>
    public void AddMsg(OpponentMsg msg)
    {
        _msgQ.Enqueue(msg);
    }

    private void Proc_Msg()
    {
        if (_msgQ.TryDequeue(out var msg))
        {
            switch (msg.MsgType)
            {
                case Type_OpponentMsg.ControllerSpawn:
                    var msgControllerSpawn = msg as OpponentMsg_ControllerSpawn;
                    Setup(msgControllerSpawn.Location, msgControllerSpawn.Dummy);
                    break;
                case Type_OpponentMsg.MoveStart:
                    var msgMoveStart = msg as OpponentMsg_MoveStart;
                    MoveStart(msgMoveStart.MoveDir);
                    break;
                case Type_OpponentMsg.MoveEnd:
                    var msgMoveEnd = msg as OpponentMsg_MoveEnd;
                    MoveEnd();
                    MoveFixed(new Vector2(msgMoveEnd.EndPos.X, msgMoveEnd.EndPos.Y));
                    break;
                case Type_OpponentMsg.MoveFix:
                    var msgMoveFix = msg as OpponentMsg_MoveFix;
                    MoveFixed(msgMoveFix.FixPos);
                    break;
                case Type_OpponentMsg.Dash:
                    var msgMoveDash = msg as OpponentMsg_Dash;
                    Dash(msgMoveDash.DashedPos);
                    break;
                case Type_OpponentMsg.Knockback:
                    var msgMoveKnockback = msg as OpponentMsg_Knockback;
                    Knockback(msgMoveKnockback.SrcPosition, msgMoveKnockback.Position);
                    break;
            }
        }
    }

    /// <summary>
    /// Get dummy.
    /// </summary>
    public UnitDummy GetDummy() => _dummy;

    /// <summary>
    /// Setup.
    /// </summary>
    public void Setup(DTO_Location location, UnitDummy dummy = null)
    {
        _spawnPos = new Vector2(location.Pos.X, location.Pos.Y);
        _refreshMovement = new Vector2(location.MoveDirection.X, location.MoveDirection.Y);
        transform.position = _spawnPos;

        if (dummy != null)
        {
            _dummy = dummy;
        }

        _dummy?.FixPosition(_spawnPos);
    }

    /// <summary>
    /// Updatge movement.
    /// </summary>
    public void MoveStart(System.Numerics.Vector2 moveDir)
    {
        _refreshMovement = new Vector2(moveDir.X, moveDir.Y);
    }

    /// <summary>
    /// Move end.
    /// </summary>
    public void MoveEnd()
    {
        // Reset status.
        _refreshMovement = Vector2.zero;
    }

    /// <summary>
    /// Get weapon collider.
    /// </summary>
    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }
    
    /// <summary>
    /// Get position.
    /// </summary>
    public Vector2 GetPosition () => _rb.position;

    /// <summary>
    /// Update animator.
    /// </summary>
    private void UpdateAnaimator()
    {
        _myAnimator.SetFloat("moveX", _refreshMovement.x);
        _myAnimator.SetFloat("moveY", _refreshMovement.y);
    }

    /// <summary>
    /// Move.
    /// </summary>
    private bool MoveDummy()
    {
        // Move.
        if (_moveDir != Vector2.zero)
        {
            //Debug.Log($"Move. dir: {_moveDir}, spped: {moveSpeed}, time: {Time.fixedDeltaTime}");
#if POS_SYNC_1
            _rb.MovePosition(_rb.position + _moveDir * (moveSpeed * Time.fixedDeltaTime));
#elif POS_SYNC_2
            _dummy.MoveTo(_moveDir * (moveSpeed * Time.fixedDeltaTime));
#endif
            return true;
        }

        return false;
    }

    /// <summary>
    /// Move fixed.
    /// </summary>
    public void MoveFixed(Vector2 fixPos)
    {
#if POS_SYNC_1
        _rb.MovePosition(fixPos);
#elif POS_SYNC_2
        _dummy.FixPosition(fixPos);
#endif
    }

    /// <summary>
    /// Adjust player facing.
    /// </summary>
    public void AdjustPlayerFacingDirection(float dirX)
    {
        if (_mySpriteRenderer == null)
        {
            return;
        }

        if (dirX > 0)
        {
            // 오른쪽.
            _mySpriteRenderer.flipX = false;
            FacingLeft = false;
        }
        else if (dirX < 0)
        {
            // 왼쪽
            _mySpriteRenderer.flipX = true;
            FacingLeft = true;
        }
    }

    /// <summary>
    /// Dash.
    /// </summary>
    public void Dash(Vector2 dashedPos)
    {
        // Update status.
        myTrailRenderer.emitting = true;
        _isDashing = true;

        // Start dash routine.
        StartCoroutine(EndDashRoutine(dashedPos));
    }

    /// <summary>
    /// End dash routine.
    /// </summary>
    private IEnumerator EndDashRoutine(Vector2 dashedPos)
    {
        float timePassed = 0f;
        var startPos = GetPosition();

        while (timePassed < GameData.DASH_TIME)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / GameData.DASH_TIME;
#if POS_SYNC_1
            _rb.position = Vector2.Lerp(startPos, dashedPos, linearT);
#elif POS_SYNC_2
            _dummy.FixPosition(Vector2.Lerp(startPos, dashedPos, linearT));
#endif

            yield return null;
        }

        myTrailRenderer.emitting = false;

        _isDashing = false;

        yield return new WaitForSeconds(GameData.DASH_COOLDOWN);
    }

    /// <summary>
    /// Knocking back.
    /// </summary>
    public void Knockback(System.Numerics.Vector2 srcPos, System.Numerics.Vector2 knockbackPos)
    {
        // Knockback.
        //_knockback.ForceKnockback(srcPos, knockbackPos, GameData.KNOCKBACK_THRUST);

        // flash.
        if (_flash)
        {
            StartCoroutine(_flash.FlashRoutine());
        }

        //_isMoving = false;
    }
}
