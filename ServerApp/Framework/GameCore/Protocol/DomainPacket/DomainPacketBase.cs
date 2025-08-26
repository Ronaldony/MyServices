using MemoryPack;
using System;

namespace GameCore.GamePacket
{
    /// <summary>
    /// DomainPacketBase_CS.
    /// </summary>
    public class DomainPacketBase_CS
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }
    }

    /// <summary>
    /// DomainPacketBase_SC.
    /// </summary>
    public abstract class DomainPacketBase_SC : IDisposable
    {
        public abstract void Dispose();
    }
}
