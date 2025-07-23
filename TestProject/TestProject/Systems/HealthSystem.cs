using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Systems
{
    /// <summary>
    /// HealthSystem.
    /// </summary>
    public class HealthSystem
    {
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;
        public float HealthPercentage => MaxHealth > 0 ? CurrentHealth / MaxHealth : 0;

        public HealthSystem(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void SetHealthPercent(float percent)
        {
            CurrentHealth = MaxHealth * Math.Clamp(percent, 0f, 1f);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
    }
}
