namespace TestProject.Datas
{
    // ===== 14. 분산 시스템 확장 (미래 확장성) =====
    /// <summary>
    /// IDistributedGameNode.
    /// </summary>
    public interface IDistributedGameNode
    {
        Task<bool> CanHandlePlayer(int playerId);
        Task TransferPlayer(int playerId, IDistributedGameNode targetNode);
        Task<PlayerState> GetPlayerState(int playerId);
    }
}
