using TestProject.Managers;
using TestProject.Objects;
using TestProject.Threats;
using TestProject.Types;
using TestProject.Utils;

namespace TestProject.Systems
{
    /// <summary>
    /// CombatSystem.
    /// </summary>
    public class CombatSystem
    {
        private readonly Creature _owner;

        public ThreatManager ThreatManager { get; }
        public IWorldObject CurrentTarget { get; private set; }
        public bool IsInCombat { get; private set; }
        public float AttackRange { get; set; } = 2f;
        public float AttackSpeed { get; set; } = 2f; // Attacks per second
        public float BaseDamage { get; set; } = 25f;

        private float _lastAttackTime;

        public CombatSystem(Creature owner)
        {
            _owner = owner;
            ThreatManager = new ThreatManager();
        }

        public void SetTarget(IWorldObject target)
        {
            CurrentTarget = target;
            if (target != null)
            {
                ThreatManager.AddThreat(target, 100f); // Initial aggro
            }
        }

        public void EnterCombat()
        {
            IsInCombat = true;
            // Notify nearby friendly creatures
            CallForHelp();
        }

        public void ResetCombat()
        {
            IsInCombat = false;
            CurrentTarget = null;
        }

        public void ClearAllThreat()
        {
            ThreatManager.ClearAllThreat();
        }

        public bool CanAttack()
        {
            if (CurrentTarget == null) return false;

            float timeSinceLastAttack = Time.time - _lastAttackTime;
            return timeSinceLastAttack >= (1f / AttackSpeed);
        }

        public void PerformAttack(IWorldObject target)
        {
            if (!CanAttack()) return;

            _lastAttackTime = Time.time;

            // Calculate damage
            float damage = BaseDamage + UnityEngine.Random.Range(-5f, 5f);

            // Apply damage (would use proper damage system in real implementation)
            ApplyDamage(target, damage);

            // Generate threat
            ThreatManager.AddThreat(target, damage * 1.5f);

            // Trigger combat events
            OnAttackPerformed?.Invoke(_owner, target, damage);
        }

        private void ApplyDamage(IWorldObject target, float damage)
        {
            // In real implementation, would interact with health system
            if (target is Player player)
            {
                player.Health.TakeDamage(damage);

                // Add threat to target's threat table if they're a creature
                if (player.CurrentTarget is Creature targetCreature)
                {
                    targetCreature.Combat.ThreatManager.AddThreat(_owner, damage);
                }
            }
        }

        private void CallForHelp()
        {
            var nearbyCreatures = WorldManager.Instance.GetObjectsInRange(_owner.Position, 20f)
                .OfType<Creature>()
                .Where(c => c != _owner && c.AI.AIType == _owner.AI.AIType);

            foreach (var creature in nearbyCreatures)
            {
                if (creature.AI.CurrentState == AIState.Idle && CurrentTarget != null)
                {
                    creature.Combat.ThreatManager.AddThreat(CurrentTarget, 50f);
                    creature.AI.TransitionToState(AIState.Combat);
                }
            }
        }

        public event Action<IWorldObject, IWorldObject, float> OnAttackPerformed;
    }
}
