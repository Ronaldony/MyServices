namespace ServerEngine.Config.Database
{
    /// <summary>
    /// ConsulDatabase.
    /// </summary>
    public class Config_MySQLConnection
    {
        /// <summary>
        /// DB Alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// DB Connection string.
        /// </summary>
        public string ConnectString { get; set; }
    }
}
