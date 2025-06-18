using GameCore;
using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool GettingKnockedBack { get; private set; }

    // Callback.
    public delegate void OnKnockBack(Vector2 knockBackPos);
    public event OnKnockBack KnockBack;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Vector3 dmgSrcPosition, float knockBackThrust)
    {
        GettingKnockedBack = true;

        // mass: ����, mass�� ����Ͽ� �˹�ǰ� �ϱ� ����.
        Vector2 momentum = (transform.position - dmgSrcPosition).normalized * knockBackThrust * rb.mass;

        StartCoroutine(KnockRoutine(rb.position + (momentum * GameData.KNOCKBACK_TIME)));

        rb.AddForce(momentum, ForceMode2D.Impulse);

        Debug.Log($"KnockBack Mass: {rb.mass}, normalized: {(transform.position - dmgSrcPosition).normalized}, difference: {momentum}");

    }
    private IEnumerator KnockRoutine(Vector2 knockBackPos)
    {
        if (null != KnockBack)
        {
            KnockBack(knockBackPos);
        }

        yield return new WaitForSeconds(GameData.KNOCKBACK_TIME);

        Debug.Log($"calc: {knockBackPos}, now: {rb.position}");

        rb.velocity = Vector2.zero;
        GettingKnockedBack = false;
    }
}
