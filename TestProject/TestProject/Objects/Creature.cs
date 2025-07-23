using System.Numerics;
using TestProject.AI;
using TestProject.Creatures;
using TestProject.Entity;
using TestProject.Systems;
using TestProject.Types;

namespace TestProject.Objects
{
    /// <summary>
    /// Creature.
    /// </summary>
    public class Creature : IWorldObject
    {
        public EntityId Id { get; }
        public Vector3 Position { get; set; }
        public ObjectType ObjectType => ObjectType.Creature;
        public bool IsInWorld { get; set; }
        public bool IsDead { get; set; }

        // AI Systems
        public AIBrain AI { get; }
        public DetectionSystem Detection { get; }
        public MovementSystem Movement { get; }
        public CombatSystem Combat { get; }
        public HealthSystem Health { get; }

        // Creature Data
        public string Name { get; set; }
        public int Level { get; set; }
        public CreatureTemplate Template { get; set; }

        public Creature(EntityId id, CreatureTemplate template, Vector3 spawnPosition)
        {
            Id = id;
            Template = template;
            Position = spawnPosition;
            IsInWorld = true;

            // Initialize systems
            AI = new AIBrain(this);
            Detection = new DetectionSystem(this);
            Movement = new MovementSystem(this);
            Combat = new CombatSystem(this);
            Health = new HealthSystem(template.MaxHealth);

            // Configure from template
            ApplyTemplate(template);

            // Set spawn position
            AI.SpawnPosition = spawnPosition;
        }

        private void ApplyTemplate(CreatureTemplate template)
        {
            Name = template.Name;
            Level = template.Level;
            AI.AIType = template.AIType;
            AI.LeashRange = template.LeashRange;

            Detection.SightRange = template.SightRange;
            Detection.AggroRange = template.AggroRange;
            Detection.FieldOfView = template.FieldOfView;

            Movement.MovementSpeed = template.MovementSpeed;
            Movement.RunSpeed = template.RunSpeed;

            Combat.AttackRange = template.AttackRange;
            Combat.AttackSpeed = template.AttackSpeed;
            Combat.BaseDamage = template.BaseDamage;
        }

        public void Update(float deltaTime)
        {
            if (!IsInWorld || Health.IsDead) return;

            Detection.Update(deltaTime);
            AI.Update(deltaTime);
            Movement.Update(deltaTime);
        }

        public Vector3 GetForwardVector()
        {
            // Simplified - would use actual rotation in real implementation
            return Vector3.UnitZ;
        }
    }
}
