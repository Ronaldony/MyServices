namespace GameCore.Packet
{
    /// <summary>
    /// Packet_Login.
    /// </summary>
    public class Packet_Login
    {
        public string Uid { get; set; }

        public string PId { get; set; }

        public string Token { get; set; }
    }
}
