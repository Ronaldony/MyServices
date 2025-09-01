namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using Server_Realm.Hubs.Msg;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 우편물 수령.
        /// </summary>
        private void RetrievePost(WorldMsg_RetrievePost worldMsg)
        {
            // 월드에 플레이어 추가.
            if (false == _worldGroup.PlayerUnitDict.TryGetValue(worldMsg.Pid, out var pu))
            {
                // 다른 존으로 이동했거나, 게임 종료된 플레이어.
                worldMsg.TryCount++;

                if (worldMsg.TryCount < GameData.POST_RETRIEVE_COUNT)
                {
                    // 우편물 수령 재시도.
                    _zoneRouter.RouteToZone(worldMsg.Pid, worldMsg);
                }
                else
                {
                    // 우편물 수령 실패.
                    _playHubProvider.Tell<HubMsg_FailRetrievePost>(new HubMsg_FailRetrievePost
                    {
                        PostUid = worldMsg.Post.PostUid,
                        TargetPid = worldMsg.Pid
                    });
                }

                return;
            }

            // Inventory 처리.
            pu.RetrievePost(worldMsg.Post);
        }
    }
}
