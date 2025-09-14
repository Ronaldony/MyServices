using GameCore;
using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    // Callback.
    public delegate void OnKnockBack(Vector2 knockBackPos);
    public event OnKnockBack KnockBack;

    private Rigidbody2D rb;
    private bool _isKnockBack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Force knockback.
    /// </summary>
    public void ForceKnockback(Vector3 srcPos, float knockBackThrust)
    {
        _isKnockBack = true;

        var direction = (transform.position - srcPos).normalized;
        Vector2 momentum = direction * knockBackThrust * rb.mass;

        StartCoroutine(KnockRoutine(rb.position + (momentum * GameData.KNOCKBACK_TIME)));

        rb.AddForce(momentum, ForceMode2D.Impulse);

        Debug.Log($"KnockBack - hit pos: {srcPos} Mass: {rb.mass}, normalized: {direction}, difference: {momentum}");
    }

    /// <summary>
    /// Force knockback.
    /// </summary>
    public void ForceKnockback(System.Numerics.Vector2 srcPos, System.Numerics.Vector2 knockbackPos, float knockBackThrust)
    {
        _isKnockBack = true;

        var convertSrcPos = new Vector3(srcPos.X, srcPos.Y);

        var direction = (transform.position - convertSrcPos).normalized;
        Vector2 momentum = direction * knockBackThrust * rb.mass;

        StartCoroutine(KnockRoutine(rb.position + (momentum * GameData.KNOCKBACK_TIME)));

        // Force knockback.
        rb.AddForce(momentum, ForceMode2D.Impulse);

        // Fix position.
        rb.position = new Vector2(knockbackPos.X, knockbackPos.Y);

        Debug.Log($"KnockBack Mass: {rb.mass}, normalized: {direction}, difference: {momentum}");
    }

    /// <summary>
    /// Is knockback.
    /// </summary>
    public bool IsKnockback()
    {
        return _isKnockBack;
    }

    private IEnumerator KnockRoutine(Vector2 knockBackPos)
    {
        if (null != KnockBack)
        {
            KnockBack(knockBackPos);
        }

        yield return new WaitForSeconds(GameData.KNOCKBACK_TIME);

        //rb.velocity = Vector2.zero;
        _isKnockBack = false;
    }
}
