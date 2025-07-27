using System.Numerics;

namespace Server_Realm.Worlds.Update
{
    /// <summary>
    /// WorldUpdate_AttackUnit.
    /// </summary>
    public class WorldUpdate_AttackUnit : IDisposable
    {
        public WorldUpdate_AttackUnit()
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
