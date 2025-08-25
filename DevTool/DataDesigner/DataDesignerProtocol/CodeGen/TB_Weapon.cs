namespace GameCore
{
    using System;
    using GameCore;

    public class TB_Weapon : ITableBase
    {
        /// <summary>
        /// Schema ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// damage
        /// </summary>
        public int Damage { get; set; }
        /// <summary>
        /// cool time
        /// </summary>
        public int CoolTime { get; set; }
        /// <summary>
        /// collision type
        /// </summary>
        public Type_Collision CollisionType { get; set; }
        /// <summary>
        /// projectile typ
        /// </summary>
        public Type_Projectile ProjectileType { get; set; }
    }
}