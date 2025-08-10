using GameCore;
using UnityEngine;

public class MobPathfinding : MonoBehaviour
{
    [SerializeField] private int MobId;

    private Rigidbody2D _rb;

    private float _velocity;

    private Vector2 _moveDirection;

    private Knockback _knockBack;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    { 
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _knockBack = GetComponent<Knockback>();
        _rb = GetComponent<Rigidbody2D>();

        var tbMob = TDBManagement.Instance.GetById<TB_Mob>(MobId);

        _velocity = tbMob.Speed;
    }

    private void FixedUpdate()
    {
        // Status knockback.
        if (_knockBack.GettingKnockedBack) { return; }

        // check next position.
        //_rb.position = Vector2.MoveTowards(_rb.position, _nextPos, _velocity * Time.fixedDeltaTime);
        _rb.position += _moveDirection * _velocity * Time.fixedDeltaTime;


        if (_moveDirection.x < 0) { _spriteRenderer.flipX = true; }
        else if (_moveDirection.x > 0) { _spriteRenderer.flipX = false; }
    }

    /// <summary>
    /// Move to.
    /// </summary>
    public void MoveTo(System.Numerics.Vector2 nextDir)
    {
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
    /// Get position.
    /// </summary>
    public Vector2 GetPosition() => _rb.position;
}
