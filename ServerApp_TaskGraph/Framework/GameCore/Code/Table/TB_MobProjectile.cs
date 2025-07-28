namespace GameCore
{
    using System;
    using GameCore;

    public class TB_MobProjectile : ITableBase
    {
        /// <summary>
        /// Schema ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Radius { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type_MobProjectile ProjectileType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxRange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Velocity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsPenetrates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsGrenade { get; set; }
    }
}