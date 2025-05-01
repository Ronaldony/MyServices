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
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Attack { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Range { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CoolTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type_Collision CollisionType { get; set; }
    }
}