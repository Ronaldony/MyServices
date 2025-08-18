using GameCore;
using Network;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool _isDead { get; private set; }

    private int _maxHealth;
    private float _damageRecoveryTime = 1f;

    private Slider _healthSlider;
    private int _currentHealth;
    private bool _canTakeDamage = true;
    private Knockback _knockback;
    private Flash _flash;

    private GamePacketManager _gpm;

    const string HEALTH_SLIDER_TEXT = "Health Slider";
    const string TOWN_TEXT = "Scene1";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        _flash = GetComponent<Flash>();
        _knockback = GetComponent<Knockback>();

        _gpm = GamePacketManager.Instance;
    }

    private void Start()
    {
        var playerDataManager = PlayerDataManager.Instance;

        _isDead = false;
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
            TakeDamage(mobAI.GetTBMob().Damage, collision.transform);
        }
    }

    /// <summary>
    /// Setup.
    /// </summary>
    public void Setup(int maxHealth, float recoveryTime)
    {
        _maxHealth = maxHealth;
        _damageRecoveryTime = recoveryTime;
    }

    /// <summary>
    /// Heal player.
    /// </summary>
    public void HealPlayer()
    {
        if (_currentHealth < _maxHealth)
        {
            _currentHealth += 1;
            UpdateHealthSlider();
        }
    }

    /// <summary>
    /// Take damage.
    /// </summary>
    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!_canTakeDamage || _isDead) { return; }

        // Screen snake.
        ScreenSnakeManager.Instance.SnakeScreen();

        // Knockback.
        _knockback.GetKnockedBack(hitTransform.position, GameData.KNOCKBACK_THRUST);

        // flash.
        StartCoroutine(_flash.FlashRoutine());

        _currentHealth -= damageAmount;
        _canTakeDamage = false;
        StartCoroutine(DamageRecoveryRoutine());

        UpdateHealthSlider();
        CheckIfPlayerDeath();
    }

    /// <summary>
    /// Check player death.
    /// </summary>
    private void CheckIfPlayerDeath()
    {
        if (_currentHealth <= 0)
        {
            // Process death.
            _isDead = true;
            Destroy(ActiveWeapon.Instance.gameObject);

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
        Destroy(gameObject);
        SceneManager.LoadScene(TOWN_TEXT);
    }

    /// <summary>
    /// Damage recovery routine.
    /// </summary>
    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(_damageRecoveryTime);
        _canTakeDamage = true;
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
