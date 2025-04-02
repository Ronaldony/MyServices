using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float _velocity = 1f;

    private Rigidbody2D _rb;

    private Vector2 _nextPos;
    private Vector2 _nextDir;

    private Knockback _knockBack;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    { 
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _knockBack = GetComponent<Knockback>();
        _rb = GetComponent<Rigidbody2D>();

        _nextPos = _rb.position;
    }

    private void FixedUpdate()
    {
        // 넉백중인 경우
        if (_knockBack.GettingKnockedBack) { return; }

        // 다음 이동 위치와 같을 때까지 이동.
        if (false == _rb.position.Equals(_nextPos))
        {
            _rb.position = Vector2.MoveTowards(_rb.position, _nextPos, _velocity * Time.fixedDeltaTime);
        }

        if (_nextDir.x < 0) { _spriteRenderer.flipX = true; }
        else if (_nextDir.x > 0) { _spriteRenderer.flipX = false; }
    }

    /// <summary>
    /// Move to.
    /// </summary>
    public void MoveTo(Vector2 nextPos)
    {
        _nextPos = nextPos;
        _nextDir = nextPos.normalized;
    }

    /// <summary>
    /// Stop moving.
    /// </summary>
    public void StopMoving()
    {
        _nextPos = Vector3.zero;
    }

    /// <summary>
    /// Initialize spawn.
    /// </summary>
    public void Initialize_Spawn(int velocity)
    {
        _velocity = velocity;
    }
}
