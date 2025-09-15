using Externals.Serializer;
using Externals.UDP;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace GamePackets
{ 
    public class GamePacketManager : Singleton<GamePacketManager>
    {
        [SerializeField] private string _address;
        [SerializeField] private int _port;

        private UdpClient _client;
        private UdpPacket _sendPacket;

        public GamePacketManager()
        {
            _sendPacket = new UdpPacket();

            _client = new UdpClient();
            _client.Initialize_Handlers<GamePacketHandler>();
        }

        public int GetPing()
        {
            return _client.GetPing();
        }

        /// <summary>
        /// Connect to server.
        /// </summary>
        public bool Connect()
        {
            return _client.Connect(_address, _port);
        }

        /// <summary>
        /// Disconnect to server.
        /// </summary>
        public void Disconnect()
        {
            _client.Disconnect();
        }

        /// <summary>
        /// Send.
        /// </summary>
        public bool Send<T>(int pakcetCode, T packet) where T : GamePacketBase_CS
        {
            packet.Pid = PlayerDataManager.Instance.Pid;

            var bytes = MemoryPackDataSerializer.Serialize<T>(packet);

            _sendPacket.PacketCode = pakcetCode;
            _sendPacket.Data = bytes;

            return _client.Send(_sendPacket);
        }
    }
}
