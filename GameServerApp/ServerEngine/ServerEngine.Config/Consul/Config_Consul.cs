namespace ServerEngine.Config.Consul
{
    public class Config_Consul : ConfigBase
    {
        public string Address { get; set; }
        public string Token { get; set; }
        public string Auth { get; set; }
        public string Datacenter { get; set; }
    }
}
