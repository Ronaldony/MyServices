using System.Collections.Immutable;

namespace TestProject.Datas
{
    // 이벤트 정의 (Event Sourcing)
    public abstract record GameEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public long SequenceNumber { get; init; }
    }

    public record EntitySpawnedEvent(Guid EntityId, EntityType Type, Vector3 Position, ImmutableDictionary<string, object> Properties) : GameEvent;
    public record EntityMovedEvent(Guid EntityId, Vector3 FromPosition, Vector3 ToPosition) : GameEvent;
    public record CollisionOccurredEvent(Guid Entity1Id, Guid Entity2Id, Vector3 CollisionPoint) : GameEvent;
    public record EntityDespawnedEvent(Guid EntityId) : GameEvent;
}
