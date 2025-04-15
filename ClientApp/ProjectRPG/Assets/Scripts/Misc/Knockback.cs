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

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        GettingKnockedBack = true;

        // mass: 질량, mass에 비례하여 넉백되게 하기 위함.
        Vector2 momentum = (transform.position - damageSource.position).normalized * knockBackThrust * rb.mass;

        StartCoroutine(KnockRoutine(rb.position + (momentum * GameData.KNOCKBACK_TIME)));

        rb.AddForce(momentum, ForceMode2D.Impulse);

        Debug.Log($"KnockBack Mass: {rb.mass}, normalized: {(transform.position - damageSource.position).normalized}, difference: {momentum}");

    }
    private IEnumerator KnockRoutine(Vector2 knockBackPos)
    {
        KnockBack(knockBackPos);

        yield return new WaitForSeconds(GameData.KNOCKBACK_TIME);

        Debug.Log($"calc: {knockBackPos}, now: {rb.position}");

        rb.velocity = Vector2.zero;
        GettingKnockedBack = false;
    }
}
