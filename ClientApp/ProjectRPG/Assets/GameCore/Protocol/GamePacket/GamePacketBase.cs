using MemoryPack;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketBase_CS.
    /// </summary>
    public class GamePacketBase_CS
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// Unit id.
        /// </summary>
        [MemoryPackOrder(1)]
        public long UnitId { get; set; }
    }

    /// <summary>
    /// GamePacketBase_SC.
    /// </summary>
    public class GamePacketBase_SC : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
