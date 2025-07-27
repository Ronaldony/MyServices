using System.Diagnostics.Contracts;
using MemoryPack;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;
    using GameCore.Datas.Projectiles;

    /// <summary>
    /// GamePacket_UpdateSectionAttack.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacket_UpdateSectionAttack : GamePacketBase_SC
    {
        public GamePacket_UpdateSectionAttack()
        {
            Projectiles = new List<DTO_Projectile>();
        }

        /// <summary>
        /// Projectiles.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_Projectile> Projectiles { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public new void Dispose()
        {
            Projectiles.Clear();
        }
    }
}
