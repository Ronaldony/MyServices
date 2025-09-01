using MemoryPack;
using System;

namespace GameCore.HubPacket
{
    /// <summary>
    /// HubPacketBase_CS.
    /// </summary>
    public class HubPacketBase_CS
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }
    }

    /// <summary>
    /// HubPacketBase_SC.
    /// </summary>
    public abstract class HubPacketBase_SC : IDisposable
    {
        public abstract void Dispose();
    }
}
