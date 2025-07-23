using System.Numerics;
using TestProject.Entity;
using TestProject.Systems;
using TestProject.Types;

namespace TestProject.Objects
{
    // Placeholder Player class for reference
    public class Player : IWorldObject
    {
        public EntityId Id { get; }
        public Vector3 Position { get; set; }
        public ObjectType ObjectType => ObjectType.Player;
        public bool IsInWorld { get; set; }
        public bool IsDead { get; set; }
        public HealthSystem Health { get; }
        public IWorldObject CurrentTarget { get; set; }

        public Player(EntityId id)
        {
            Id = id;
            Health = new HealthSystem(1000f);
            IsInWorld = true;
        }
    }
}
