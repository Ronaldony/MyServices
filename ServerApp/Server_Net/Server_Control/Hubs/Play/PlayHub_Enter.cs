namespace Server_Control.Hubs.Play
{
    using GameCore;
    using GameCore.Datas.Player;
    using GameCore.HubPacket;
    using Server_Control.Hubs.Msg;
    using Server_Control.Hubs.Play.Datas;
    using ServerEngine.Core.Util;

    /// <summary>
    /// PlayHub.
    /// </summary>
    public partial class PlayHub
    {
        /// <summary>
        /// Enter
        /// </summary>
        private async Task EnterAsync(PlayHubDataSet dataSet, HubMsg_Enter msg)
        {
            var pid = dataSet.Pid;

            dataSet.IsEnter = true;
            dataSet.PostInfo = await _dbPostInfo.SelectAsync(pid);

            // 우편물 검사.
            CheckPosts(dataSet.PostInfo);

            // 우편물 정보 전송.
            SendPacket<HubPacketSC_Enter>(pid, HubPacketCode.SC_Enter, new HubPacketSC_Enter
            {
                PostInfo = dataSet.PostInfo,
            });

        }

        /// <summary>
        /// Leave
        /// </summary>
        private void Leave(PlayHubDataSet dataSet, HubMsg_Leave msg)
        {
            _hubGroup.DataSets.Remove(dataSet.Pid);
        }

        /// <summary>
        /// Enter zone.
        /// </summary>
        private void EnterZone(PlayHubDataSet dataSet, HubMsg_EnterZone msg)
        {
            dataSet.ZoneId = msg.ZoneId;
        }

        /// <summary>
        /// Check posts.
        /// </summary>
        private void CheckPosts(DTO_PostInfo postInfo)
        {
            var deletePostUids = new List<string>();
            var nowTime = TimeUtil.Now;

            foreach (var post in postInfo.Posts)
            {
                // 15일이 지난 우편 기록 삭제.
                if (TimeUtil.IsPastTime(post.Value.RegTime.AddDays(15), nowTime))
                {
                    deletePostUids.Add(post.Key);
                }
            }

            // 오랜 우편물 삭제.
            foreach (var postUid in deletePostUids)
            {
                postInfo.Posts.Remove(postUid);
            }
        }
    }
}
