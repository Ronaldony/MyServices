namespace GameCore
{
    using System;
    using GameCore;

    public class CONF_Area : ITableBase
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
        public Type_Area Area { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MinX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MinY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
    }
}