using TestProject.Types;

namespace TestProject.Creatures
{
    /// <summary>
    /// CreatureTemplate.
    /// </summary>
    public class CreatureTemplate
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public float MaxHealth { get; set; }
        public AIType AIType { get; set; }

        // Detection Settings
        public float SightRange { get; set; } = 25f;
        public float AggroRange { get; set; } = 15f;
        public float FieldOfView { get; set; } = 180f;
        public float LeashRange { get; set; } = 40f;

        // Movement Settings
        public float MovementSpeed { get; set; } = 5f;
        public float RunSpeed { get; set; } = 8f;

        // Combat Settings
        public float AttackRange { get; set; } = 2f;
        public float AttackSpeed { get; set; } = 2f;
        public float BaseDamage { get; set; } = 25f;
    }
}
