using TestProject.Architectures;

namespace TestProject.Datas
{
    /// <summary>
    /// IGameWorld.
    /// </summary>
    public interface IGameWorld
    {
        IEnumerable<ICollidable> GetActiveEntities();

        void RecordHit(int ownerId, int id, float damage);

        WorldPlayer GetPlayer(int playerId);
    }
}
