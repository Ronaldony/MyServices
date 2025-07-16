using System.Numerics;

namespace TestProject.Datas
{
    /// <summary>
    /// DamageInfo.
    /// </summary>
    public class DamageInfo
    {
        public int AttackerId { get; set; }
        public float Damage { get; set; }
        public Vector3 Position { get; set; }
        public bool IsCritical { get; set; }
        public Type_Damage DamageType { get; set; }
    }
}
