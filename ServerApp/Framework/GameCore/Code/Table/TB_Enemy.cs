namespace GameCore
{
    using System;
    using GameCore;

    public class TB_Enemy
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
        public int ItemCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
    }
}