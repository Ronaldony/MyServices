namespace Server_Realm.Zones.HuntZone
{
    using Framework.Core.Hubs;
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
            // 월드에 플레이어 추가.
            if (false == _worldGroup.PlayerUnitDict.TryGetValue(worldMsg.Pid, out var pu))
            {
                // 다른 존으로 이동했거나, 게임 종료된 플레이어.
                _hubActorProvider.Tell<PlayHubMsg_FailClaimPost>(new PlayHubMsg_FailClaimPost
                {
                    Pid = worldMsg.Pid,
                    PostUid = worldMsg.Post.PostUid
                });

                return;
            }

            // 우편물 수령.
            pu.ClaimPost(worldMsg.Post);
        }
    }
}
