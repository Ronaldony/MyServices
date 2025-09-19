namespace Server_Realm.Zones.HuntZone
{
    using Framework.Core.Hubs;
    using GameCore;
    using GameCore.GamePacket;
    using GameCore.HubPacket;
    using Microsoft.Extensions.Logging;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 우편물 수령.
        /// </summary>
        private void ClaimPost(WorldMsg_ClaimPost worldMsg)
        {
            // 플레이어가 Zone에 존재하는지 확인.
            if (false == _worldGroup.PlayerUnitDict.TryGetValue(worldMsg.Pid, out var pu))
            {
                // 다른 존으로 이동했거나, 게임 종료된 플레이어.
                _hubActorProvider.Tell<ControlMsg_ClaimPostFail>(new ControlMsg_ClaimPostFail
                {
                    Pid = worldMsg.Pid,
                    PostUid = worldMsg.Post.PostUid,
                    FailCount = worldMsg.TryCount
                });

                return;
            }

            // 우편물 수령.
            pu.ClaimPost(worldMsg.Post);

            // 우편물 수령 완료 전달.
            _hubActorProvider.Tell<ControlMsg_ClaimPostComplete>(new ControlMsg_ClaimPostComplete
            {
                Pid = worldMsg.Pid,
                PostUid = worldMsg.Post.PostUid,
            });

            // 패킷 전송
            var packetClaimPost = _memoryPoolService.Rent<GamePacketSC_ClaimPost>();
            packetClaimPost.PostUid = worldMsg.Post.PostUid;

            SendPacket(pu.Pid, GamePacketCode.SC_ClaimPost, packetClaimPost);

            // release.
            _memoryPoolService.Release(packetClaimPost);
        }
    }
}
