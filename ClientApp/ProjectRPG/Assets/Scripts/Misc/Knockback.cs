using GameCore;
using System.Collections;
using UnityEngine;
using static Knockback;

public class Knockback : MonoBehaviour
{
    public bool GettingKnockedBack { get; private set; }

    // Callback.
    public delegate void OnKnockBack();
    public event OnKnockBack KnockBack;
    public delegate void OnFinishKnockBack();
    public event OnFinishKnockBack FinishKnockBack;

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
        rb.AddForce(momentum, ForceMode2D.Impulse);

        Debug.Log($"KnockBack Mass: {rb.mass}, normalized: {(transform.position - damageSource.position).normalized}, difference: {momentum}");

        StartCoroutine(KnockRoutine());
    }
    private IEnumerator KnockRoutine()
    {
        KnockBack();

        yield return new WaitForSeconds(GameData.KNOCKBACK_TIME);

        FinishKnockBack();

        rb.velocity = Vector2.zero;
        GettingKnockedBack = false;
    }
}
