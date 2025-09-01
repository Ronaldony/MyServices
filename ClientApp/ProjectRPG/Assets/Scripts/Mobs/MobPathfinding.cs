using GameCore;
using UnityEngine;

public class MobPathfinding : MonoBehaviour
{
    private Rigidbody2D _rb;

    private float _velocity;

    private Vector2 _moveDirection = new Vector2();

    private Knockback _knockBack;
    private SpriteRenderer _spriteRenderer;

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
        // Status knockback.
        if (_knockBack.GettingKnockedBack) { return; }

        // check next position.
        _rb.position += _moveDirection * _velocity * Time.fixedDeltaTime;

        //Debug.Log($"Path finding. now pos: {_rb.position}, dir: {_moveDirection}");

        if (_moveDirection.x < 0) { _spriteRenderer.flipX = true; }
        else if (_moveDirection.x > 0) { _spriteRenderer.flipX = false; }
    }

    /// <summary>
    /// Move to.
    /// </summary>
    public void UpdateMovement(System.Numerics.Vector2 nextDir, System.Numerics.Vector2 pos)
    {
        _rb.position = new Vector2(pos.X, pos.Y);
        _moveDirection = new Vector2(nextDir.X, nextDir.Y);
    }

    /// <summary>
    /// Stop moving.
    /// </summary>
    public void StopMoving()
    {
        _moveDirection = Vector3.zero;
    }

    /// <summary>
    /// Now position.
    /// </summary>
    public Vector2 NowPos()
    {
        return _rb.position;
    }

    /// <summary>
    /// Get position.
    /// </summary>
    public Vector2 GetPosition() => _rb.position;
}
