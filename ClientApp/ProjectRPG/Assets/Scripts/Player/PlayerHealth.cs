using GameCore;
using GameCore.GamePacket;
using GamePackets;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool _isDead { get; private set; }

    private int _maxHealth;

    private Slider _healthSlider;
    private int _currentHealth;
    private Knockback _knockback;
    private Flash _flash;

    const string HEALTH_SLIDER_TEXT = "MyHealthSlider";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        _flash = GetComponent<Flash>();
        _knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        _isDead = false;

        var playerDataManager = PlayerDataManager.Instance;

        _maxHealth = playerDataManager.PlayInfo.Stat.HealthMax;
        _currentHealth = playerDataManager.PlayInfo.Stat.Health;

        // Slider.
        UpdateHealthSlider();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var mobAI = collision.gameObject.GetComponent<MobAI>();

        if (mobAI)
        {
            TakeDamage(collision.transform);
        }
    }

    /// <summary>
    /// Heal player.
    /// </summary>
    public void HealPlayer(int heal)
    {
        _currentHealth += heal;

        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }

        UpdateHealthSlider();
    }

    /// <summary>
    /// Take damage.
    /// </summary>
    public void TakeDamage(Transform hitTransform)
    {
        if (_isDead) { return; }

        // Screen snake.
        ScreenSnakeManager.Instance.SnakeScreen();

        // Knockback.
        //_knockback.ForceKnockback(hitTransform.position, GameData.KNOCKBACK_THRUST);

        // Send packet.
        GamePacketManager.Instance.Send<GamePacketCS_KnockBack>(
            GamePacketCode.CS_KnockBack,
            new GamePacketCS_KnockBack
            {
                Position = new System.Numerics.Vector2(transform.position.x, transform.position.y)
            });

        // flash.
        StartCoroutine(_flash.FlashRoutine());
    }

    public void UpdateHealth(int health)
    {
        _currentHealth = health;

        UpdateHealthSlider();

        CheckPlayerDeath();
    }

    /// <summary>
    /// Check player death.
    /// </summary>
    private void CheckPlayerDeath()
    {
        if (_currentHealth <= 0)
        {
            // Process death.
            _isDead = true;
            
            // Disable weapon.
            ActiveWeapon.Instance.gameObject.SetActive(false);
            //Destroy(ActiveWeapon.Instance.gameObject);

            _currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }

    /// <summary>
    /// Death load scene.
    /// </summary>
    private IEnumerator DeathLoadSceneRoutine()
    {
        yield return new WaitForSeconds(2f);

        // Respawn player.
        //Destroy(gameObject);
    }

    /// <summary>
    /// Update health slider.
    /// </summary>
    private void UpdateHealthSlider()
    {
        if (_healthSlider == null)
        {
            _healthSlider = GameObject.Find(HEALTH_SLIDER_TEXT).GetComponent<Slider>();
        }

        _healthSlider.maxValue = _maxHealth;
        _healthSlider.value = _currentHealth;
    }
}
