using GameCore;
using System.Collections;
using UnityEngine;

public class OpponentController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;

    private Vector2 _spawnPos;
    private Vector2 _refreshMovement;
    private Rigidbody2D _rb;
    private Animator _myAnimator;
    private SpriteRenderer _mySpriteRenderer;
    private Knockback _knockback;
    private Flash _flash;

    private float _startingMoveSpeed;

    private Vector2 _moveDir;
    private bool _isMoving;
    private bool _isDashing;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _myAnimator = GetComponent<Animator>();
        _mySpriteRenderer = GetComponent<SpriteRenderer>();
        _knockback = GetComponent<Knockback>();
        _flash = GetComponent<Flash>();

        _startingMoveSpeed = moveSpeed;

        _rb.position = _spawnPos;
    }

    private void Update()
    {
        UpdateAnaimator();
    }

    private void FixedUpdate()
    {
        // 넉백 혹은 죽음인 경우 처리 넘김.
        if (_knockback.IsKnockback() || PlayerHealth.Instance._isDead)
        {
            return;
        }

        if (_isDashing)
        {
            _rb.MovePosition(_rb.position + _refreshMovement * (moveSpeed * Time.fixedDeltaTime));
            return;
        }

        // Adjust face 방향.
        AdjustPlayerFacingDirection(_refreshMovement.x);

        // Pre move.
        MovePre();

        // Move
        if (Move())
        {
            MovePost();
        }
    }

    /// <summary>
    /// Setup.
    /// </summary>
    public void Setup(float x, float y, System.Numerics.Vector2 moveDir)
    {
        _spawnPos = new Vector2(x, y);
        _refreshMovement = new Vector2(moveDir.X, moveDir.Y);
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
        _isMoving = false;

        //Debug.Log($"Move Stop - DateTIme: {packetTime}, DeltaTime: {_moveDeltaTime}");
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
    public Vector2 GetPosition() => _rb.position;

    /// <summary>
    /// Update animator.
    /// </summary>
    private void UpdateAnaimator()
    {
        _myAnimator.SetFloat("moveX", _refreshMovement.x);
        _myAnimator.SetFloat("moveY", _refreshMovement.y);
    }

    /// <summary>
    /// Before move.
    /// </summary>
    private void MovePre()
    {
        // check moving.
        if (_isMoving)
        {
            // Cond 1. not moving anymore.
            // Cond 2. change move direction.
            if ((_refreshMovement == Vector2.zero) || (_refreshMovement != _moveDir))
            {
                MoveEnd();
            }
        }

        // 이전 방향 값 갱신.
        //_preDirection = new Vector2((float)Math.Round(_movement.x, 2), (float)Math.Round(_movement.y, 2));
        _moveDir = _refreshMovement;
    }

    /// <summary>
    /// Move.
    /// </summary>
    private bool Move()
    {
        // Move.
        if (_moveDir != Vector2.zero)
        {
            _rb.MovePosition(_rb.position + _moveDir * (moveSpeed * Time.fixedDeltaTime));
            //Debug.Log($"rb: {rb.position}, DeltaTime: {Time.fixedDeltaTime}, result: {movement * (moveSpeed * Time.fixedDeltaTime)}, movement: {movement}");

            return true;
        }

        return false;
    }

    /// <summary>
    /// After move.
    /// </summary>
    private void MovePost()
    {
        _isMoving = true;
        //Debug.Log($"Move End - Dir: {_moveDirection}, DateTIme: {packetTime}, DeltaTime: {_moveDeltaTime}");
    }

    /// <summary>
    /// Move fixed.
    /// </summary>
    public void MoveFixed(float x, float y)
    {
        _rb.position = new Vector2(x, y);
    }

    /// <summary>
    /// Adjust player facing.
    /// </summary>
    private void AdjustPlayerFacingDirection(float dirX)
    {
        if (dirX > 0)
        {
            // 왼쪽.
            _mySpriteRenderer.flipX = false;
        }
        else if (dirX < 0)
        {
            // 오른쪽
            _mySpriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// Dash.
    /// </summary>
    public void Dash(Vector2 dashedPos)
    {
        // Set movement status.
        moveSpeed *= dashSpeed;
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
        yield return new WaitForSeconds(GameData.DASH_TIME);
        moveSpeed = _startingMoveSpeed;
        myTrailRenderer.emitting = false;

        //Debug.Log($"Dashed position. pos: {dashedPos}");

        _isDashing = false;

        // Fix position
        MoveFixed(dashedPos.x, dashedPos.y);

        yield return new WaitForSeconds(GameData.DASH_COOLDOWN);
    }

    /// <summary>
    /// Knocking back.
    /// </summary>
    public void Knockback(System.Numerics.Vector2 srcPos, System.Numerics.Vector2 knockbackPos)
    {
        // Knockback.
        _knockback.ForceKnockback(srcPos, knockbackPos, GameData.KNOCKBACK_THRUST);

        // flash.
        StartCoroutine(_flash.FlashRoutine());

        _isMoving = false;
    }
}
