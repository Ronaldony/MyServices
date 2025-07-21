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
    /// GamePacketSC_UpdateSectionProjectile.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_UpdateSectionProjectile : GamePacketBase_SC, IDisposable
    {
        public GamePacketSC_UpdateSectionProjectile()
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
        public void Dispose()
        {
            Projectiles.Clear();
        }
    }
}
