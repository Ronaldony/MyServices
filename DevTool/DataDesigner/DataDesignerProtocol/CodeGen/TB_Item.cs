namespace GameCore
{
    using System;
    using GameCore;

    public class TB_Item : ITableBase
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
        /// Type_Item
        /// </summary>
        public Type_Item ItemType { get; set; }
        /// <summary>
        /// Item rate
        /// </summary>
        public int RateRef { get; set; }
    }
}