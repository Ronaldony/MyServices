namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Microsoft.Extensions.Logging;
    using Server_Realm.Entities;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 무기 교체.
        /// </summary>
        private void SwitchWeapon(PlayerUnit pu, GamePacketCS_SwitchWeapon packet)
        {
            _logger.LogDebug($"SwitchWeapon");

            pu.SwitchWeapon(packet.WeaponId);

            // 패킷 전송 - 이동 시작.
            pu.WorldSection.SendPacket_AroundSections(pu.UnitId, GamePacketCode.SC_SwitchWeapon, new GamePacketSC_SwitchWeapon
            {
                UnitId = pu.UnitId,
                WeaponId = packet.WeaponId,
            });
        }
    }
}
