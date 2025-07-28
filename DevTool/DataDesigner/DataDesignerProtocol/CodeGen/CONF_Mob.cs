namespace GameCore
{
    using System;
    using GameCore;

    public class CONF_Mob : ITableBase
    {
        /// <summary>
        /// Schema ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type_Area Area { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SpawnTimeSec { get; set; }
    }
}