using MemoryPack;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketSC_UpdateSectionMove.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_UpdateSectionMove : GamePacketBase_SC
    {
        public GamePacketSC_UpdateSectionMove()
        {
            PlayInfos = new List<DTO_PlayInfo>();
            Enemies = new List<DTO_Enemy>();
        }

        /// <summary>
        /// Play info list.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// Enemies.
        /// </summary>
        [MemoryPackOrder(1)]
        public List<DTO_Enemy> Enemies { get; set; }

        public new void Dispose()
        {
            PlayInfos.Clear(); 
            Enemies.Clear();
        }
    }
}
