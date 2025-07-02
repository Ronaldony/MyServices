namespace ServerEngine.Config.Strapi
{
    /// <summary>
    /// Configuration for connection to Consul.
    /// </summary>
    public class Config_Strapi : ConfigBase
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
        /// ContentType.
        /// </summary>
        public string ContentType { get; set; }
    }
}
