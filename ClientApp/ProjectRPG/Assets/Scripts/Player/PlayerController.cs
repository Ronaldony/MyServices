using GameCore;
using GameCore.NetPacket;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get; set; }

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private Knockback knockback;
    private float startingMoveSpeed;

    private Type_MoveDirection _moveDirection;

    private bool _isMoving;
    private bool _isDashing;

    // Data Manger.
    private NetPacketManager _netPacketManager;
    private PlayerDataManager _playerDataManager;

    private float _deltaTimeSec;


    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();

        _netPacketManager = NetPacketManager.Instance;
        _playerDataManager = PlayerDataManager.Instance;

        // Setup position.
        rb.position = new Vector2(_playerDataManager.PlayInfo.Position.Pos.X, _playerDataManager.PlayInfo.Position.Pos.Y);
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
        startingMoveSpeed = moveSpeed;

        ActiveInventory.Instance.EquipStartingWeapon();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
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
        else if(vectorDir.x > 0)
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

    private void Move()
    {
        if (knockback.GettingKnockedBack || PlayerHealth.Instance.isDead) { return; }

        // Update move.
        if (_moveDirection != Type_MoveDirection.None)
        {
            rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));

            Debug.Log($"rb: {rb.position}, DeltaTime: {Time.fixedDeltaTime}, result: {movement * (moveSpeed * Time.fixedDeltaTime)}, movement: {movement}");

            _deltaTimeSec += Time.fixedDeltaTime;
        }

        // Send MoveStart.
        if ((_moveDirection != Type_MoveDirection.None) && (false == _isMoving))
        {
            _isMoving = true;

            // Send MoveEnd.
            _netPacketManager.Send<NetPacketCS_MoveStart>(
                NetPacketCode.CS_MoveStart,
                new NetPacketCS_MoveStart
                {
                    MoveDirection = _moveDirection,
                    Pid = _playerDataManager.Pid,
                    PacketTime = DateTime.Now
                });
        }

        // 이동 방향 확인.
        var moveDirection = GetMoveDirection(movement.normalized);

        // 이동 방향 검사.
        if (_isMoving)
        {
            // Cond 1. 이동 정지한 경우.
            // Cond 2. 현재 이동 방향과 이전 이동 방향이 다른 경우.
            if ((moveDirection == Type_MoveDirection.None) || (moveDirection != _moveDirection))
            {
                _playerDataManager.PlayInfo.Position.Pos = new System.Numerics.Vector2(rb.position.x, rb.position.y);

                // 이동 방향이 바뀐 경우, MoveEnd를 전송.
                _netPacketManager.Send<NetPacketCS_MoveEnd>(
                    NetPacketCode.CS_MoveEnd,
                    new NetPacketCS_MoveEnd
                    {
                        Pid = _playerDataManager.Pid,
                        EndPos = _playerDataManager.PlayInfo.Position.Pos,
                        PacketTime = DateTime.Now,
                        DeltaTimeSec = _deltaTimeSec
                    });

                Debug.Log($"DeltaTime: {_deltaTimeSec}");

                // initialize status.
                _isMoving = false;
                _deltaTimeSec = 0;
            }
        }

        // 이동 방향 갱신.
        _moveDirection = moveDirection;
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRenderer.flipX = true;
            FacingLeft = true;
        }
        else
        {
            mySpriteRenderer.flipX = false;
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
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCD);
        _isDashing = false;
    }
}
