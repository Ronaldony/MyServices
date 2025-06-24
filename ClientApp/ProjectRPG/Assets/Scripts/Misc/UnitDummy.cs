using UnityEngine;

public class UnitDummy : MonoBehaviour
{
    private ulong _enemyUid;

    private Rigidbody2D _rb2d;

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Get enemy uid.
    /// </summary>
    public ulong GetUid()
    {
        return _enemyUid;
    }

    /// <summary>
    /// Initialize.
    /// </summary>
    public void Initialize(ulong enemyUid, System.Numerics.Vector2 pos)
    {
        _enemyUid = enemyUid;
        _rb2d.position = new Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// Move.
    /// </summary>
    public void Move(System.Numerics.Vector2 pos)
    {
        _rb2d.position = new Vector2(pos.X, pos.Y);
    }
}
