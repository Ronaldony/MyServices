using GameCore;
using GameCore.GamePacket;
using GamePackets;
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

    private Vector2 _moveDirection;
    private bool _isMoving;
    private bool _isDashing;
    private bool _isDashCoolDown;

    // Data Manger.
    private GamePacketManager _gamePacketManager;
    private PlayerDataManager _playerDataManager;

    private ConcurrentQueue<UpdateGamePacket> _updatePacketQ; 

    protected override void Awake()
    {
        base.Awake();

        _playerControls = new PlayerControls();
        _rb = GetComponent<Rigidbody2D>();
        _myAnimator = GetComponent<Animator>();
        _mySpriteRenderer = GetComponent<SpriteRenderer>();
        _knockback = GetComponent<Knockback>();

        _gamePacketManager = GamePacketManager.Instance;
        _playerDataManager = PlayerDataManager.Instance;

        _updatePacketQ = new ConcurrentQueue<UpdateGamePacket>();

        // Setup position.
        _rb.position = new Vector2(_playerDataManager.PlayInfo.Location.Pos.X, _playerDataManager.PlayInfo.Location.Pos.Y);

        // Knockback.
        //_knockback.KnockBack += OnKnockBack;

        // Spawn units.
        GamePacketManager.Instance.Send<GamePacketCS_EnterPlayer>(GamePacketCode.CS_EnterPlayer, new GamePacketCS_EnterPlayer());
    }

    private void Start()
    {
        // Player controls.
        _playerControls.Combat.Dash.performed += _ => OnDash();
        _playerControls.Inventory.Toggle.performed += _ => OnToggleInventory();
        _playerControls.PostBox.Toggle.performed += _ => OnTogglePostBox();

        _startingMoveSpeed = moveSpeed;

        PlayerManagement.Instance.SetPlayerObject(gameObject);
    }

    public void EnableControls()
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
                case GamePacketCode.SC_MoveFixed:
                    {
                        var pakcetData = updatePacket.Data as GamePacketSC_MoveFixed;
                        MoveFixed(pakcetData.FixedPos.X, pakcetData.FixedPos.Y);
                    }
                    break;
            }
        }
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
            _rb.MovePosition(_rb.position + _movement * (moveSpeed * Time.fixedDeltaTime));
            return;
        }

        // Update movement.
        _movement = _playerControls.Movement.Move.ReadValue<Vector2>();

        AdjustPlayerFacingDirection();

        // 이동 처리.
        MovePre();
        Move();
        MovePost();
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
    private void MovePre()
    {
        // check moving.
        if (_isMoving)
        {
            // Cond 1. not moving anymore.
            // Cond 2. change move direction.
            if ((_movement.normalized == Vector2.zero) || (_movement.normalized != _moveDirection))
            {
                MoveStop();
            }
        }

        // �̵� ���� ����.
        var moveDir = _movement.normalized;

        _moveDirection = new Vector2((float)Math.Round(moveDir.x, 2), (float)Math.Round(moveDir.y, 2));
    }

    /// <summary>
    /// Move.
    /// </summary>
    private void Move()
    {
        // Update move.
        if (_moveDirection != Vector2.zero)
        {
            _rb.MovePosition(_rb.position + _movement * (moveSpeed * Time.fixedDeltaTime));
            _moveDeltaTime += Time.fixedDeltaTime;
            //Debug.Log($"rb: {rb.position}, DeltaTime: {Time.fixedDeltaTime}, result: {movement * (moveSpeed * Time.fixedDeltaTime)}, movement: {movement}");
        }
    }

    /// <summary>
    /// After move.
    /// </summary>
    private void MovePost()
    {
        // Send MoveStart.
        if ((_moveDirection != Vector2.zero) && (false == _isMoving))
        {
            MoveStart();
        }
    }

    /// <summary>
    /// Move stop.
    /// </summary>
    private void MoveStop()
    {
        PlayerDataManager.Instance.PlayInfo.Location.Pos = new System.Numerics.Vector2(_rb.position.x, _rb.position.y);

        var packetTime = DateTime.Now.AddSeconds(Time.fixedDeltaTime);

        // Send MoveEnd.
        _gamePacketManager.Send<GamePacketCS_MoveEnd>(
            GamePacketCode.CS_MoveEnd,
            new GamePacketCS_MoveEnd
            {
                EndPos = PlayerDataManager.Instance.PlayInfo.Location.Pos,
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
        _gamePacketManager.Send<GamePacketCS_MoveStart>(
            GamePacketCode.CS_MoveStart,
            new GamePacketCS_MoveStart
            {
                Direction = new System.Numerics.Vector2(_moveDirection.x, _moveDirection.y),
                PacketTime = packetTime
            });

        //Debug.Log($"Move End - Dir: {_moveDirection}, DateTIme: {packetTime}, DeltaTime: {_moveDeltaTime}");
    }

    /// <summary>
    /// Move fixed.
    /// </summary>
    private void MoveFixed(float x, float y)
    {
        _playerDataManager.PlayInfo.Location.Pos = new System.Numerics.Vector2(x, y);
        _rb.position = new Vector2(x, y);
    }

    /// <summary>
    /// Enqueue UpdatePacket.
    /// </summary>
    public void AddUpdatePacket(UpdateGamePacket updatePacket)
    {
        _updatePacketQ.Enqueue(updatePacket);
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

            _gamePacketManager.Send<GamePacketCS_Dash>(
                GamePacketCode.CS_Dash,
                new GamePacketCS_Dash
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
        _gamePacketManager.Send<GamePacketCS_MoveStart>(
            GamePacketCode.CS_MoveStart,
            new GamePacketCS_MoveStart
            {
                Direction = new System.Numerics.Vector2(_moveDirection.x, _moveDirection.y),
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
        _playerDataManager.PlayInfo.Location.Pos = new System.Numerics.Vector2(_rb.position.x, _rb.position.y);

        // Send packet.
        _gamePacketManager.Send<GamePacketCS_KnockBack>(
            GamePacketCode.CS_KnockBack,
            new GamePacketCS_KnockBack
            {
                Position = new System.Numerics.Vector2(knockBackPos.x, knockBackPos.y)
            });

        _isMoving = false;
        _moveDeltaTime = 0;
    }

    private void OnToggleInventory()
    {
        Inventory.Instance.Toggle();
    }

    private void OnTogglePostBox()
    {
        PostBox.Instance.Toggle();
    }
}
