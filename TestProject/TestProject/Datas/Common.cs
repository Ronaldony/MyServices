namespace TestProject.Datas
{
    /// <summary>
    /// SectionId.
    /// </summary>
    public readonly record struct SectionId(int X, int Y)
    {
        public IEnumerable<SectionId> GetNeighbors(int radius = 1)
        {
            for (int x = X - radius; x <= X + radius; x++)
            {
                for (int y = Y - radius; y <= Y + radius; y++)
                {
                    if (x == X && y == Y) continue;
                    yield return new SectionId(x, y);
                }
            }
        }
    }

    // 플레이어 상태 (불변)
    /// <summary>
    /// PlayerState.
    /// </summary>
    public record PlayerState(
        int PlayerId,
        Vector3 Position,
        Vector3 Velocity,
        SectionId Section,
        int Health,
        int Mana,
        DateTime LastUpdate,
        bool IsActive = true
    )
    {
        public const float MaxSpeed = 10.0f;
        public const float CollisionRadius = 1.0f;
    }

    // 게임 이벤트들
    public abstract record GameEvent(DateTime Timestamp, int PlayerId);
    public record PlayerMoveEvent(DateTime Timestamp, int PlayerId, Vector3 From, Vector3 To) : GameEvent(Timestamp, PlayerId);
    public record CollisionEvent(DateTime Timestamp, int PlayerId, int OtherPlayerId, Vector3 Point) : GameEvent(Timestamp, PlayerId);
    public record SectionChangeEvent(DateTime Timestamp, int PlayerId, SectionId From, SectionId To) : GameEvent(Timestamp, PlayerId);
    public record PlayerJoinEvent(DateTime Timestamp, int PlayerId, Vector3 SpawnPoint) : GameEvent(Timestamp, PlayerId);
    public record PlayerLeaveEvent(DateTime Timestamp, int PlayerId, string Reason) : GameEvent(Timestamp, PlayerId);

    // 네트워크 메시지
    public record MoveRequest(int PlayerId, Vector3 TargetPosition, float DeltaTime, int SequenceId);
    public record MoveResponse(int PlayerId, Vector3 Position, bool IsValid, int SequenceId);

    public record SectionUpdateBatch(SectionId SectionId, PlayerMoveEvent[] Events, DateTime Timestamp);
}
