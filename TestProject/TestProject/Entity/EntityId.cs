namespace TestProject.Entity
{
    /// <summary>
    /// EntityId.
    /// </summary>
    public readonly struct EntityId : IEquatable<EntityId>
    {
        public readonly uint Value;
        public EntityId(uint value) => Value = value;
        public bool Equals(EntityId other) => Value == other.Value;
        public override int GetHashCode() => (int)Value;
        public static implicit operator uint(EntityId id) => id.Value;
        public static implicit operator EntityId(uint value) => new(value);
    }
}
