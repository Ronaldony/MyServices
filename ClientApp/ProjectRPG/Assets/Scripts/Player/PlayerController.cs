using GameCore;
using GameCore.NetPacket;
using Network;
using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get; set; }

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;

    private PlayerControls _playerControls;
    private Vector2 _movement;
    private Rigidbody2D _rb;
    private Animator _myAnimator;
    private SpriteRenderer _mySpriteRenderer;
    private Knockback _knockback;

    private float _startingMoveSpeed;
    private float _moveDeltaTime;

    private Type_MoveDirection _moveDirection;
    private bool _isMoving;
    private bool _isDashing;

    // Data Manger.
    private NetPacketManager _netPacketManager;
    private PlayerDataManager _playerDataManager;

    private ConcurrentQueue<UpdatePacket> _updatePacketQ; 

    protected override void Awake()
    {
        base.Awake();

        _playerControls = new PlayerControls();
        _rb = GetComponent<Rigidbody2D>();
        _myAnimator = GetComponent<Animator>();
        _mySpriteRenderer = GetComponent<SpriteRenderer>();
        _knockback = GetComponent<Knockback>();

        _netPacketManager = NetPacketManager.Instance;
        _playerDataManager = PlayerDataManager.Instance;

        _updatePacketQ = new ConcurrentQueue<UpdatePacket>();

        // Setup position.
        _rb.position = new Vector2(_playerDataManager.PlayInfo.Position.Pos.X, _playerDataManager.PlayInfo.Position.Pos.Y);
    }

    private void Start()
    {
        _playerControls.Combat.Dash.performed += _ => Dash();
        _startingMoveSpeed = moveSpeed;

        ActiveInventory.Instance.EquipStartingWeapon();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void Update()
    {
        UpdateAnaimator();
        Proc_UpdatePacket();
    }

    private void Proc_UpdatePacket()
    {
        // Process update pakcet.
        while (_updatePacketQ.TryDequeue(out var updatePacket))
        {
            switch (updatePacket.PacketCode)
            {
                case NetPacketCode.SC_MoveFixed:
                    {
                        var pakcetData = updatePacket.Data as NetPacketSC_MoveFixed;
                        MoveFixed(pakcetData.FixedPos.X, pakcetData.FixedPos.Y);
                    }
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        // Update movement.
        _movement = _playerControls.Movement.Move.ReadValue<Vector2>();

        AdjustPlayerFacingDirection();

        // 플레이어가 넉백 상태이거나 죽은 경우 이동 처리하지 않음.
        if (_knockback.GettingKnockedBack || PlayerHealth.Instance.isDead)
        {
            MoveStop();
        }
        else
        {
            // 이동 처리.
            BeforeMove();
            Move();
            AfterMove();
        }
    }

    /// <summary>
    /// Get weapon collider.
    /// </summary>
    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    /// <summary>
    /// Update animator.
    /// </summary>
    private void UpdateAnaimator()
    {
        _myAnimator.SetFloat("moveX", _movement.x);
        _myAnimator.SetFloat("moveY", _movement.y);
    }

    /// <summary>
    /// Before move.
    /// </summary>
    private void BeforeMove()
    {
        // 이동 방향 확인.
        var moveDirection = GetMoveDirection(_movement.normalized);

        // 이동 방향 검사.
        if (_isMoving)
        {
            // Cond 1. 이동 정지한 경우.
            // Cond 2. 현재 이동 방향과 이전 이동 방향이 다른 경우.
            if ((moveDirection == Type_MoveDirection.None) || (moveDirection != _moveDirection))
            {
                MoveStop();
            }
        }

        // 이동 방향 갱신.
        _moveDirection = moveDirection;
    }

    /// <summary>
    /// Move.
    /// </summary>
    private void Move()
    {
        // Update move.
        if (_moveDirection != Type_MoveDirection.None)
        {
            _rb.MovePosition(_rb.position + _movement * (moveSpeed * Time.fixedDeltaTime));

            //Debug.Log($"rb: {rb.position}, DeltaTime: {Time.fixedDeltaTime}, result: {movement * (moveSpeed * Time.fixedDeltaTime)}, movement: {movement}");
            
            _moveDeltaTime += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// After move.
    /// </summary>
    private void AfterMove()
    {
        // Send MoveStart.
        if ((_moveDirection != Type_MoveDirection.None) && (false == _isMoving))
        {
            MoveStart();
        }
    }

    /// <summary>
    /// Move stop.
    /// </summary>
    private void MoveStop()
    {
        _playerDataManager.PlayInfo.Position.Pos = new System.Numerics.Vector2(_rb.position.x, _rb.position.y);

        var pakcetTime = DateTime.Now.AddSeconds(Time.fixedDeltaTime);

        // 이동 방향이 바뀐 경우, MoveEnd를 전송.
        _netPacketManager.Send<NetPacketCS_MoveEnd>(
            NetPacketCode.CS_MoveEnd,
            new NetPacketCS_MoveEnd
            {
                Pid = _playerDataManager.Pid,
                EndPos = _playerDataManager.PlayInfo.Position.Pos,
                PacketTime = pakcetTime,
            });

        //Debug.Log($"Move Stop - DateTIme: {pakcetTime}, DeltaTime: {_moveDeltaTime}");

        // initialize status.
        _isMoving = false;
        _moveDeltaTime = 0;
    }

    /// <summary>
    /// Move start.
    /// </summary>
    private void MoveStart()
    {
        _isMoving = true;

        var pakcetTime = DateTime.Now.AddSeconds(_moveDeltaTime);

        // Send MoveEnd.
        _netPacketManager.Send<NetPacketCS_MoveStart>(
            NetPacketCode.CS_MoveStart,
            new NetPacketCS_MoveStart
            {
                MoveDirection = _moveDirection,
                Pid = _playerDataManager.Pid,
                PacketTime = pakcetTime
            });

        //Debug.Log($"Move End - Dir: {_moveDirection}, DateTIme: {pakcetTime}, DeltaTime: {_moveDeltaTime}");
    }

    /// <summary>
    /// Move fixed.
    /// </summary>
    private void MoveFixed(float x, float y)
    {
        _playerDataManager.PlayInfo.Position.Pos = new System.Numerics.Vector2(x, y);
        _rb.position = new Vector2(x, y);
    }

    /// <summary>
    /// Enqueue UpdatePacket.
    /// </summary>
    public void EnqueueUpdatePacket(UpdatePacket updatePacket)
    {
        _updatePacketQ.Enqueue(updatePacket);
    }

    /// <summary>
    /// Get move direction.
    /// </summary>
    private Type_MoveDirection GetMoveDirection(Vector2 vectorDir)
    {
        if ((vectorDir.x == 0) && (vectorDir.y == 0))
        {
            // None.
            return Type_MoveDirection.None;
        }
        else if (vectorDir.x > 0)
        {
            if (vectorDir.y == 0) { return Type_MoveDirection.Right; }          // Right.
            else if (vectorDir.y > 0) { return Type_MoveDirection.RightTop; }   // Right Top.
            else { return Type_MoveDirection.RightDown; }                       // Right Down.
        }
        else if (vectorDir.x < 0)
        {
            if (vectorDir.y == 0) { return Type_MoveDirection.Left; }          // Left.
            else if (vectorDir.y > 0) { return Type_MoveDirection.LeftTop; }   // Left Top.
            else { return Type_MoveDirection.LeftDown; }                       // Left Down.
        }
        else if (vectorDir.y > 0)
        {
            if (vectorDir.x == 0) { return Type_MoveDirection.Top; }            // Top.
            else if (vectorDir.x > 0) { return Type_MoveDirection.RightTop; }   // Right Top.
            else { return Type_MoveDirection.LeftTop; }                         // Left Top.
        }
        else // if (vectorDir.y < 0)
        {
            if (vectorDir.x == 0) { return Type_MoveDirection.Down; }            // Down.
            else if (vectorDir.x > 0) { return Type_MoveDirection.RightDown; }   // Right Down.
            else { return Type_MoveDirection.LeftDown; }                         // Left Down.
        }
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            _mySpriteRenderer.flipX = true;
            FacingLeft = true;
        }
        else
        {
            _mySpriteRenderer.flipX = false;
            FacingLeft = false;
        }
    }

    private void Dash()
    {
        if (!_isDashing && Stamina.Instance.CurrentStamina > 0)
        {
            Stamina.Instance.UseStamina();

            _isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = .25f;

        yield return new WaitForSeconds(dashTime);
        moveSpeed = _startingMoveSpeed;
        myTrailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCD);
        _isDashing = false;
    }
}
