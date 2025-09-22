using GameCore;
using GameCore.Datas.Plays;
using UnityEngine;

public class MobPathfinding : MonoBehaviour
{
    private Rigidbody2D _rb;

    private float _velocity;

    private Vector2 _moveDirection = new Vector2();
    private Vector2 _endPos;
    private Vector2 _spawnPos;

    private Knockback _knockBack;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    { 
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _knockBack = GetComponent<Knockback>();
        _rb = GetComponent<Rigidbody2D>();

        _endPos = _rb.position;

        var mobAI = GetComponent<MobAI>();
        var tbMob = TDBManagement.Instance.GetById<TB_Mob>(mobAI.GetMobId());

        _velocity = tbMob.Speed;
    }

    private void FixedUpdate()
    {
        // Status knockback.
        //if (_knockBack.IsKnockback()) { return; }

        MoveToNext();

        //Debug.Log($"Path finding. now pos: {_rb.position}, dir: {_moveDirection}");

        if (_moveDirection.x < 0) { _spriteRenderer.flipX = true; }
        else if (_moveDirection.x > 0) { _spriteRenderer.flipX = false; }
    }

    /// <summary>
    /// Check move end.
    /// </summary>
    private bool IsMoveEnd()
    {
        // 조건 1. 이동 가능 범위 검사.
        if (Vector2.Distance(_rb.position, _spawnPos) > GameData.MOB_RETURN_DISTANCE + 2)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Move to.
    /// </summary>
    public void UpdateMovement(DTO_Location location)
    {
        _moveDirection = new Vector2(location.MoveDirection.X, location.MoveDirection.Y);
        _rb.position = new Vector2(location.Pos.X, location.Pos.Y);
        _endPos = new Vector2(location.PosEnd.X, location.PosEnd.Y);
    }

    /// <summary>
    /// Stop moving.
    /// </summary>
    public void StopMoving()
    {
        _moveDirection = Vector3.zero;
    }

    /// <summary>
    /// Fix position.
    /// </summary>
    public void FixPosition(float x, float y)
    {
        _rb.position = new Vector2(x, y);
    }

    /// <summary>
    /// Setup.
    /// </summary>
    public void Setup(System.Numerics.Vector2 spawnPos)
    {
        _spawnPos = new Vector2(spawnPos.X, spawnPos.Y);
    }

    /// <summary>
    /// Get position.
    /// </summary>
    public Vector2 GetPosition() => _rb.position;

    /// <summary>
    /// Move to next.
    /// </summary>
    private void MoveToNext()
    {
        // check next position.
        _rb.position += _moveDirection * _velocity * Time.fixedDeltaTime;

        if (_moveDirection != Vector2.zero)
        {
            if (IsMoveEnd())
            {
                StopMoving();
            }
        }
    }
}
