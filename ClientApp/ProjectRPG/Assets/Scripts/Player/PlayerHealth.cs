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
    private int _defense;

    private Slider _healthSlider;
    private int _currentHealth;
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
        _isDead = false;

        var playerDataManager = PlayerDataManager.Instance;

        _maxHealth = playerDataManager.PlayInfo.Stat.HealthMax;
        _currentHealth = playerDataManager.PlayInfo.Stat.Health;
        _defense = playerDataManager.PlayInfo.Stat.Defense;

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
        if (_isDead) { return; }

        // Screen snake.
        ScreenSnakeManager.Instance.SnakeScreen();

        // Knockback.
        //_knockback.GetKnockedBack(hitTransform.position, GameData.KNOCKBACK_THRUST);

        // flash.
        StartCoroutine(_flash.FlashRoutine());

        var damage = damageAmount - _defense;

        if (damage > 0)
        {
            _currentHealth -= damage;
        }
        else
        {
            // Damage가 0이하인 경우.
            _currentHealth--;
        }

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
