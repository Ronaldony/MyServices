namespace GameCore
{
    using System;
    using GameCore;

    public class TB_Mob : ITableBase
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
        public int Damage { get; set; }
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
        public int Speed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Defense { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type_MobAttack AttackType { get; set; }
    }
}