using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.HubPacket;
    using Server_Realm.Entities;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 아이템 소모 처리.
        /// </summary>
        private void ConsumeItem(PlayerUnit pu, GamePacketCS_ConsumeItem packet)
        {
            var tbItem = _tdbService.GetById<TB_Item>(packet.RefId);
            if (null == tbItem)
            {
                _logger.LogError($"not exist item. refId: {packet.RefId}");
                return;
            }

            // Consume item.
            if (pu.ConsumeItem(tbItem))
            {
                // 패킷 전송 - 이동 시작.
                pu.WorldSection.SendPacket_AroundSections(pu.UnitId, GamePacketCode.SC_ConsumeItem, new GamePacketSC_ConsumeItem
                {
                    RefId = packet.RefId,
                });
            }
        }
    }
}
