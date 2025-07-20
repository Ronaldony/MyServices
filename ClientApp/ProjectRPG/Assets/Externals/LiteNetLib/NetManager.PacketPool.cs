using System;

namespace LiteNetLib
{
    public partial class NetManager
    {
        private GamePacket _poolHead;
        private int _poolCount;
        private readonly object _poolLock = new object();

        /// <summary>
        /// Maximum packet pool size (increase if you have tons of packets sending)
        /// </summary>
        public int PacketPoolSize = 1000;

        public int PoolCount => _poolCount;
        
        private GamePacket PoolGetWithData(PacketProperty property, byte[] data, int start, int length)
        {
            int headerSize = GamePacket.GetHeaderSize(property);
            GamePacket packet = PoolGetPacket(length + headerSize);
            packet.Property = property;
            Buffer.BlockCopy(data, start, packet.RawData, headerSize, length);
            return packet;
        }

        //Get packet with size
        private GamePacket PoolGetWithProperty(PacketProperty property, int size)
        {
            GamePacket packet = PoolGetPacket(size + GamePacket.GetHeaderSize(property));
            packet.Property = property;
            return packet;
        }

        private GamePacket PoolGetWithProperty(PacketProperty property)
        {
            GamePacket packet = PoolGetPacket(GamePacket.GetHeaderSize(property));
            packet.Property = property;
            return packet;
        }

        internal GamePacket PoolGetPacket(int size)
        {
            if (size > NetConstants.MaxPacketSize)
                return new GamePacket(size);

            GamePacket packet;
            lock (_poolLock)
            {
                packet = _poolHead;
                if (packet == null)
                    return new GamePacket(size);
                
                _poolHead = _poolHead.Next;
                _poolCount--;
            }
            
            packet.Size = size;
            if (packet.RawData.Length < size)
                packet.RawData = new byte[size];
            return packet;
        }

        internal void PoolRecycle(GamePacket packet)
        {
            if (packet.RawData.Length > NetConstants.MaxPacketSize || _poolCount >= PacketPoolSize)
            {
                //Don't pool big packets. Save memory
                return;
            }
            
            //Clean fragmented flag
            packet.RawData[0] = 0;
            lock (_poolLock)
            {
                packet.Next = _poolHead;
                _poolHead = packet;
                _poolCount++;
            }
        }
    }
}
