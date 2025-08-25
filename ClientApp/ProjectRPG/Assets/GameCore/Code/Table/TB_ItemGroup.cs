namespace GameCore
{
    using System;

    public class TB_ItemGroup : ITableBase
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
        public int GroupId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ItemRefId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RateWeight { get; set; }
    }
}