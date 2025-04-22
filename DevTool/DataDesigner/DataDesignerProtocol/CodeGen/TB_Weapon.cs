namespace GameCore
{
    using System;

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
        public int Cooltime { get; set; }
    }
}