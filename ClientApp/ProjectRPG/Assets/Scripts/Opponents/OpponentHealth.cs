using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// OpponentHealth.
/// </summary>
public class OpponentHealth : MonoBehaviour
{
    private bool _isDead;

    private int _maxHealth;

    [SerializeField] private GameObject _healthBar;
    private int _currentHealth;

    readonly int DEATH_HASH = Animator.StringToHash("Death");

    private void Start()
    {
        _isDead = false;
    }

    /// <summary>
    /// Setup.
    /// </summary>
    public void Setup(int currentHealth, int maxHealth)
    {
        _currentHealth = currentHealth;
        _maxHealth = maxHealth;

        // Slider.
        UpdateHealthSlider();
    }

    /// <summary>
    /// Update health.
    /// </summary>
    public void UpdateHealth(int health)
    {
        _currentHealth = health;

        // Slide bar.
        UpdateHealthSlider();

        // Check death.
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
            //Destroy(ActiveWeapon.Instance.gameObject);

            _currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
        }
    }

    /// <summary>
    /// Update health slider.
    /// </summary>
    private void UpdateHealthSlider()
    {
        var healthSlider = _healthBar.GetComponent<Slider>();

        healthSlider.maxValue = _maxHealth;
        healthSlider.value = _currentHealth;
    }
}
