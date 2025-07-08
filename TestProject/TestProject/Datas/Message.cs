namespace TestProject.Datas
{

    // 메시지 정의
    public abstract record Message
    {
        public Guid SenderId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }

    public record SpawnMessage(EntityType Type, Vector3 Position, Dictionary<string, object> Properties) : Message;
    public record MoveMessage(Guid EntityId, Vector3 TargetPosition) : Message;
    public record UpdatePositionMessage(Guid EntityId, Vector3 NewPosition) : Message;
    public record CollisionCheckMessage(Guid EntityId, Vector3 Position, float Radius) : Message;
    public record CollisionDetectedMessage(Guid Entity1Id, Guid Entity2Id, Vector3 CollisionPoint) : Message;
    public record DespawnMessage(Guid EntityId) : Message;
    public record GetStateMessage(Guid EntityId) : Message;
    public record StateResponseMessage(EntityState State) : Message;
}
