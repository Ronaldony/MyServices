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
    private bool _isDashCoolDown;

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
        _rb.position = new Vector2(_playerDataManager.PlayInfo.Location.Position.X, _playerDataManager.PlayInfo.Location.Position.Y);

        Initialize_CallBack();
    }

    /// <summary>
    /// Initialize callback.
    /// </summary>
    private void Initialize_CallBack()
    {
        // Knockback.
        _knockback.KnockBack += OnKnockBack;
    }

    private void Start()
    {
        _playerControls.Combat.Dash.performed += _ => OnDash();
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
        // 넉백 혹은 죽음인 경우 처리 넘김.
        if (_knockback.GettingKnockedBack || PlayerHealth.Instance.isDead)
        {
            return;
        }

        if (_isDashing)
        {
            _rb.MovePosition(_rb.position + _movement * (moveSpeed * Time.fixedDeltaTime));
            return;
        }

        // Update movement.
        _movement = _playerControls.Movement.Move.ReadValue<Vector2>();

        AdjustPlayerFacingDirection();

        // 이동 처리.
        BeforeMove();
        Move();
        AfterMove();
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
        _myAnimator.SetFloat("moveX", _movement.x);
        _myAnimator.SetFloat("moveY", _movement.y);
    }

    /// <summary>
    /// Before move.
    /// </summary>
    private void BeforeMove()
    {
        // Get move direction.
        var moveDirection = GetMoveDirection(_movement.normalized);

        // �̵� ���� �˻�.
        if (_isMoving)
        {
            // Cond 1. �̵� ������ ���.
            // Cond 2. ���� �̵� ����� ���� �̵� ������ �ٸ� ���.
            if ((moveDirection == Type_MoveDirection.None) || (moveDirection != _moveDirection))
            {
                MoveStop();
            }
        }

        // �̵� ���� ����.
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
            _moveDeltaTime += Time.fixedDeltaTime;
            //Debug.Log($"rb: {rb.position}, DeltaTime: {Time.fixedDeltaTime}, result: {movement * (moveSpeed * Time.fixedDeltaTime)}, movement: {movement}");
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
        _playerDataManager.PlayInfo.Location.Position = new System.Numerics.Vector2(_rb.position.x, _rb.position.y);

        var packetTime = DateTime.Now.AddSeconds(Time.fixedDeltaTime);

        // Send MoveEnd.
        _netPacketManager.Send<NetPacketCS_MoveEnd>(
            NetPacketCode.CS_MoveEnd,
            new NetPacketCS_MoveEnd
            {
                EndPos = _playerDataManager.PlayInfo.Location.Position,
                PacketTime = packetTime,
            });

        //Debug.Log($"Move Stop - DateTIme: {packetTime}, DeltaTime: {_moveDeltaTime}");

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

        var packetTime = DateTime.Now.AddSeconds(_moveDeltaTime);

        // Send MoveEnd.
        _netPacketManager.Send<NetPacketCS_MoveStart>(
            NetPacketCode.CS_MoveStart,
            new NetPacketCS_MoveStart
            {
                MoveDirection = _moveDirection,
                PacketTime = packetTime
            });

        //Debug.Log($"Move End - Dir: {_moveDirection}, DateTIme: {packetTime}, DeltaTime: {_moveDeltaTime}");
    }

    /// <summary>
    /// Move fixed.
    /// </summary>
    private void MoveFixed(float x, float y)
    {
        _playerDataManager.PlayInfo.Location.Position = new System.Numerics.Vector2(x, y);
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

    /// <summary>
    /// Adjust player facing.
    /// </summary>
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

    /// <summary>
    /// Dash.
    /// </summary>
    private void OnDash()
    {
        if (!_isDashCoolDown && Stamina.Instance.CurrentStamina > 0)
        {
            Stamina.Instance.UseStamina();

            _isDashing = true;
            _isDashCoolDown = true;
            moveSpeed *= dashSpeed;

            // Dash.
            var dashedPosition = _rb.position + _movement * (moveSpeed * GameData.DASH_TIME);

            _netPacketManager.Send<NetPacketCS_Dash>(
                NetPacketCode.CS_Dash,
                new NetPacketCS_Dash
                {
                    Position = new System.Numerics.Vector2(dashedPosition.x, dashedPosition.y)
                });

            //Debug.Log($"Dash calc position: {dashedPosition}");

            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    /// <summary>
    /// End dash routine.
    /// </summary>
    private IEnumerator EndDashRoutine()
    {
        yield return new WaitForSeconds(GameData.DASH_TIME);
        moveSpeed = _startingMoveSpeed;
        myTrailRenderer.emitting = false;

        //Debug.Log($"Dashed position. pos: {_rb.position}, Dir: {_moveDirection}");
        //Debug.Log($"Move Start (Dash) - Dir: {_moveDirection}, DateTIme: {DateTime.Now}");

        // Send MoveStart.
        _netPacketManager.Send<NetPacketCS_MoveStart>(
            NetPacketCode.CS_MoveStart,
            new NetPacketCS_MoveStart
            {
                MoveDirection = _moveDirection,
                PacketTime = DateTime.Now
            });


        _isDashing = false;

        yield return new WaitForSeconds(GameData.DASH_COOLDOWN);
        _isDashCoolDown = false;
    }

    /// <summary>
    /// Knocking back.
    /// </summary>
    private void OnKnockBack(Vector2 knockBackPos)
    {
        _playerDataManager.PlayInfo.Location.Position = new System.Numerics.Vector2(_rb.position.x, _rb.position.y);

        if (_isMoving)
        {
            var pakcetTime = DateTime.Now.AddSeconds(Time.fixedDeltaTime);

            _netPacketManager.Send<NetPacketCS_KnockBack>(
                NetPacketCode.CS_KnockBack,
                new NetPacketCS_KnockBack
                {
                    Position = new System.Numerics.Vector2(knockBackPos.x, knockBackPos.y)
                });

            _isMoving = false;
            _moveDeltaTime = 0;
        }
    }
}
