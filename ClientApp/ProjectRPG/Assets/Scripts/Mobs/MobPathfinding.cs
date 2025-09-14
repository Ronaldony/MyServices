using GameCore;
using UnityEngine;
using UnityEngine.VFX;

public class MobPathfinding : MonoBehaviour
{
    private Rigidbody2D _rb;

    private float _velocity;

    private Vector2 _moveDirection = new Vector2();
    private Vector2 _endPos;

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
        if (_knockBack.IsKnockback()) { return; }

        MoveToNext();

        //Debug.Log($"Path finding. now pos: {_rb.position}, dir: {_moveDirection}");

        if (_moveDirection.x < 0) { _spriteRenderer.flipX = true; }
        else if (_moveDirection.x > 0) { _spriteRenderer.flipX = false; }
    }

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

    /// <summary>
    /// Check move end.
    /// </summary>
    private bool IsMoveEnd()
    {
        var isEndPosX = false;
        var isEndPosY = false;

        var nowPos = _rb.position;

        // X 위치 검사.
        if (_moveDirection.x > 0)
        {
            // 오른쪽
            if (nowPos.x >= _endPos.x)
            {
                isEndPosX = true;
            }
        }
        else
        {
            // 왼쪽
            if (nowPos.x <= _endPos.x)
            {
                isEndPosX = true;
            }
        }

        // Y 위치 검사.
        if (_moveDirection.y > 0)
        {
            // 위
            if (nowPos.y >= _endPos.y)
            {
                isEndPosY = true;
            }
        }
        else
        {
            // 아래
            if (nowPos.y <= _endPos.y)
            {
                isEndPosY = true;
            }
        }

        return isEndPosX && isEndPosY;
    }

    /// <summary>
    /// Move to.
    /// </summary>
    public void UpdateMovement(System.Numerics.Vector2 nextDir, System.Numerics.Vector2 pos, System.Numerics.Vector2 endPos)
    {
        _rb.position = new Vector2(pos.X, pos.Y);
        _moveDirection = new Vector2(nextDir.X, nextDir.Y);
        _endPos = new Vector2(endPos.X, endPos.Y);
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
    public Vector2 GetPosition() => _rb.position;
}
