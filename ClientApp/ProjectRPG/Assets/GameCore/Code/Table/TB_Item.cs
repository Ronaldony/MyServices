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
        /// <summary>
        /// 
        /// </summary>
        public int Consume_Ref1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Consume_Value1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Consume_Ref2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Consume_Value2 { get; set; }
    }
}