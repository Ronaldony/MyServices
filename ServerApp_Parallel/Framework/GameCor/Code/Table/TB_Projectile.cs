namespace GameCore
{
    using System;
    using GameCore;

    public class TB_Projectile : ITableBase
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
        /// radius
        /// </summary>
        public int Radius { get; set; }
        /// <summary>
        /// projectile type
        /// </summary>
        public Type_Projectile ProjectileType { get; set; }
        /// <summary>
        /// max range
        /// </summary>
        public int MaxRange { get; set; }
        /// <summary>
        /// velocity
        /// </summary>
        public int Velocity { get; set; }
        /// <summary>
        /// penetrates status
        /// </summary>
        public bool IsPenetrates { get; set; }
        /// <summary>
        /// summary
        /// </summary>
        public string Summary { get; set; }
    }
}