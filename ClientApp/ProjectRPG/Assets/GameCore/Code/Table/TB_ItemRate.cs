namespace GameCore
{
    using System;
    using GameCore;

    public class TB_ItemRate : ITableBase
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
        public int Rate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type_ItemRank Rank { get; set; }
    }
}