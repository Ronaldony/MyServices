using Externals.Serializer;
using Externals.UDP;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{ 
    public class GamePacketManager : Singleton<GamePacketManager>
    {
        [SerializeField] private string _address;
        [SerializeField] private int _port;

        private UdpClient _client;
        private UdpPacket _sendPacket;
        private PlayerDataManager _playerDataManger;

        public GamePacketManager()
        {
            _client = new UdpClient();
            _sendPacket = new UdpPacket();
            _playerDataManger = PlayerDataManager.Instance;
        }

        /// <summary>
        /// Connect to server.
        /// </summary>
        public bool Connect()
        {
            return _client.Connect(_address, _port);
        }

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
