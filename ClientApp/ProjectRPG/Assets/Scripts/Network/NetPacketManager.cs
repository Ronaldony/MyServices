using Externals.Serializer;
using Externals.UDP;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{ 
    public class NetPacketManager : Singleton<NetPacketManager>
    {
        [SerializeField] private string _address;
        [SerializeField] private int _port;

        private UdpClient _client;

        private UdpPacket _sendPacket;

        public NetPacketManager()
        {
            _client = new UdpClient();
            _sendPacket = new UdpPacket();
        }

        /// <summary>
        /// Connect to server.
        /// </summary>
        public bool Connect()
        {
            return _client.Connect(_address, _port);
        }

        public bool Send<T>(int pakcetCode, T data) where T : class
        {
            var bytes = MemoryPackDataSerializer.Serialize<T>(data);

            _sendPacket.PacketCode = pakcetCode;
            _sendPacket.Data = bytes;

            return _client.Send(_sendPacket);
        }
    }
}
