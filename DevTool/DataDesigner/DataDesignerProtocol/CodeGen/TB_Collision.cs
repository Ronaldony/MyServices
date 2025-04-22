namespace GameCore
{
    using System;

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
        public float Point1_x { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Point1_y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Point2_x { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Point2_y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Point3_x { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Point3_y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Point4_x { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Point4_y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
    }
}