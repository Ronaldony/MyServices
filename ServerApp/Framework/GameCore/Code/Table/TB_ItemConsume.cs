namespace GameCore
{
    using System;
    using GameCore;

    public class TB_ItemConsume : ITableBase
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
        public Type_ItemConsume ConsumeType { get; set; }
    }
}