namespace TestProject.Datas
{
    /// <summary>
    /// DistributedGameCluster.
    /// </summary>
    public class DistributedGameCluster
    {
        private readonly List<IDistributedGameNode> _nodes = new();
        private readonly ConsistentHashing _hashRing = new();

        public async Task AddNodeAsync(IDistributedGameNode node)
        {
            _nodes.Add(node);
            _hashRing.AddNode(node);
            await RebalancePlayersAsync();
        }

        public async Task RemoveNodeAsync(IDistributedGameNode node)
        {
            _nodes.Remove(node);
            _hashRing.RemoveNode(node);
            await RebalancePlayersAsync();
        }

        private async Task RebalancePlayersAsync()
        {
            // 플레이어 재분산 로직
            await Task.Delay(1);
        }
    }
}
