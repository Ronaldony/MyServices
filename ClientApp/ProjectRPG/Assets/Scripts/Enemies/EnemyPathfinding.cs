using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1f;

    private Rigidbody2D _rb;

    private Vector2 _prevPos;
    private Vector2 _nextPos;
    private Vector2 _nextDir;
    private float _roamChangeTime;
    private float _roamChangeDirSum;

    private Knockback _knockBack;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _knockBack = GetComponent<Knockback>();
        _rb = GetComponent<Rigidbody2D>();

        _prevPos = new Vector2(_rb.position.x, _rb.position.y);
        _nextPos = _prevPos;
    }

    private void FixedUpdate()
    {
        // 넉백중인 경우
        if (_knockBack.GettingKnockedBack) { return; }

        //rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        if (_roamChangeDirSum < _roamChangeTime)
        {
            _roamChangeDirSum += Time.deltaTime;
            _rb.MovePosition(Vector2.Lerp(_prevPos, _nextPos, _roamChangeDirSum * _moveSpeed / _roamChangeTime));
        }

        if (_nextDir.x < 0) { _spriteRenderer.flipX = true; }
        else if (_nextDir.x > 0) { _spriteRenderer.flipX = false; }
    }

    /// <summary>
    /// Move to.
    /// </summary>
    public void MoveTo(Vector2 nextPos, float roamChangTime)
    {
        _prevPos = _nextPos;
        _nextPos = nextPos;
        _nextDir = nextPos.normalized;
        _roamChangeTime = roamChangTime;
        _roamChangeDirSum = 0;
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
    public void Initialize_Spawn(int moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }
}
