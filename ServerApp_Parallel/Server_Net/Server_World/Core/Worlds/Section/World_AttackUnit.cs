using System.Numerics;

namespace Server_World.Core.Worlds.Section
{
    /// <summary>
    /// World_AttackUnit.
    /// </summary>
    public class World_AttackUnit : IDisposable
    {
        public World_AttackUnit()
        {
            Enemies = new Dictionary<ulong, Vector2>();
        }

        /// <summary>
        /// Enemy. 
        /// </summary>
        public Dictionary<ulong, Vector2> Enemies { get; set; }

        public void Dispose()
        {
            Enemies.Clear();
        }
    }
}
