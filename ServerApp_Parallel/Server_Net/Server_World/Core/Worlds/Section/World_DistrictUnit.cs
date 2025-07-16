
using System.Collections.Concurrent;

namespace Server_World.Core.Worlds.Section
{
    using Server_World.Core.Datas;

    /// <summary>
    /// World_DistrictUnit.
    /// </summary>
    public class World_DistrictUnit : IDisposable
    {
        public World_DistrictUnit()
        {
            WorldPlayers = new ConcurrentBag<World_Player>();
            WorldEnemies = new ConcurrentBag<World_Enemy>();
        }

        /// <summary>
        /// WorldPlayers.
        /// </summary>
        public ConcurrentBag<World_Player> WorldPlayers { get; set; }

        /// <summary>
        /// WorldEnemies.
        /// </summary>
        public ConcurrentBag<World_Enemy> WorldEnemies { get; set; }

        public void Dispose()
        {
            WorldPlayers.Clear();
            WorldEnemies.Clear();
        }
    }
}
