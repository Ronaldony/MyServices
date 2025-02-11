namespace GameCore
{
    using System;
    using GameCore;

    public class TC_GameConfig
    {
        /// <summary>
        /// Schema ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type_Item Item { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type_Player Player { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RefItem { get; set; }
    }
}