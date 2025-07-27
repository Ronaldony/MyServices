namespace GameCore
{
    using System;
    using GameCore;

    public class TB_Enemy : ITableBase
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
        public int Health { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Attack { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type_Item DropItem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AttackRange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AttackCooldown { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MoveSpeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Defense { get; set; }
    }
}