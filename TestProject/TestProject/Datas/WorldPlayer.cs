using System.Numerics;

namespace TestProject.Datas
{
    /// <summary>
    /// GamePlayer.
    /// </summary>
    public class WorldPlayer
    {
        public float DamageMultiplier { get; set; }
        public bool IsAlive { get; set; }
        public Vector3 Position { get; set; }

        public bool CanFire(int weaponId)
        {
            return false;
        }

        public bool HasAmmo(int weaponId)
        {
            return false;
        }
    }
}
