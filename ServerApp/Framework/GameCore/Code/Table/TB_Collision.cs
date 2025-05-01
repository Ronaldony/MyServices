namespace GameCore
{
    using System;
    using GameCore;

    public class TB_Collision : ITableBase
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
        public Type_Collision CollisionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float OffsetX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float OffsetY { get; set; }
    }
}