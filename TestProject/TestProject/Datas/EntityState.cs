using System.Collections.Immutable;

namespace TestProject.Datas
{
    /// <summary>
    // Immutable Entity State
    /// EntityState.
    /// </summary>
    public record EntityState
    {
        public Guid Id { get; init; }
        public EntityType Type { get; init; }
        public Vector3 Position { get; init; }
        public float CollisionRadius { get; init; }
        public ImmutableDictionary<string, object> Properties { get; init; }
        public bool IsActive { get; init; }
        public long Version { get; init; }

        public EntityState WithPosition(Vector3 newPosition) =>
            this with { Position = newPosition, Version = Version + 1 };

        public EntityState WithProperty(string key, object value) =>
            this with { Properties = Properties.SetItem(key, value), Version = Version + 1 };

        public EntityState Deactivate() =>
            this with { IsActive = false, Version = Version + 1 };
    }
}
