namespace ServerEngine.Config.Consul
{
    /// <summary>
    /// Configuration for connection to Consul.
    /// </summary>
    public class Config_Consul : ConfigBase
    {
        /// <summary>
        /// Consul Address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// API Token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Authentication for fetch.
        /// </summary>
        public string Auth { get; set; }

        /// <summary>
        /// Consule data center.
        /// </summary>
        public string Datacenter { get; set; }
    }
}
