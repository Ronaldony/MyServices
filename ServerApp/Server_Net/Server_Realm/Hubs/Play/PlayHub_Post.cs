using Microsoft.Extensions.Logging;

namespace Server_Realm.Hubs.Play
{
    using GameCore;
    using GameCore.HubPacket;
    using Server_Realm.Hubs.Msg;
    using Server_Realm.Hubs.Play.Datas;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// PlayHub.
    /// </summary>
    public partial class PlayHub
    {
        /// <summary>
        /// 우편물 푸시.
        /// </summary>
        private async Task PushPostAsync(PlayHubDataSet dataSet, HubMsg_PushPost msg)
        {
            var pid = dataSet.Pid;

            // PostInfo 로드.
            if (dataSet.PostInfo == null)
            {
                dataSet.PostInfo = await _dbPostInfo.SelectAsync(pid);
            }

            var postInfo = dataSet.PostInfo;

            // 새로운 우편물 처리.
            foreach (var post in msg.Posts[pid])
            {
                // 해당 우편물이 이미 있는 경우 무시.
                if (postInfo.Posts.ContainsKey(post.PostUid))
                {
                    continue;
                }

                // 우편물 추가.
                postInfo.Posts.Add(post.PostUid, post);
            }

            // Hub에 진입한 경우, 패킷 전송.
            if (dataSet.IsEnter)
            {
                // 새 우편물 푸시.
                SendPacket<HubPacketSC_PushPost>(pid, HubPacketCode.SC_PushPost, new HubPacketSC_PushPost
                {
                    PushPosts = msg.Posts[pid]
                });
            }
        }

        /// <summary>
        /// 우편물 수령.
        /// </summary>
        private void RetrievePost(PlayHubDataSet dataSet, HubMsg_RetrievePost msg)
        {
            var pid = dataSet.Pid;

            // PostInfo 로드.
            if (dataSet.PostInfo == null)
            {
                // 수령 실패. PostInfo가 없는 경우 비정상적인 상황. (PlayDomain 접속 시 무조건 로드되기 때문)
                SendPacket<HubPacketSC_RetrievePost>(pid, HubPacketCode.SC_RetrievePost, new HubPacketSC_RetrievePost
                {
                    IsRetrieved = false
                });

                _logger.LogError($"load PostInfo error. pid: {pid}");
                return;
            }

            // Posts.
            var postInfo = dataSet.PostInfo;
            var postUid = msg.PostUid;

            // 우편물 검사.
            if (false == postInfo.Posts.ContainsKey(postUid))
            {
                // 수령 실패
                SendPacket<HubPacketSC_RetrievePost>(pid, HubPacketCode.SC_RetrievePost, new HubPacketSC_RetrievePost
                {
                    IsRetrieved = false
                });

                _logger.LogError($"not exist post. uid: {postUid}");
                return;
            }

            // 우편물 수령.
            postInfo.Posts[postUid].IsRetrived = true;

            // 수령 확인.
            SendPacket<HubPacketSC_RetrievePost>(pid, HubPacketCode.SC_RetrievePost, new HubPacketSC_RetrievePost
            {
                IsRetrieved = true,
                PostUid = postUid
            });

            // Zone에 아이템 수령 전송.
            _zoneRouter.RouteToZone<WorldMsg_RetrievePost>(pid, new WorldMsg_RetrievePost 
            { 
                Post = postInfo.Posts[postUid]
            });
        }

        /// <summary>
        /// Delete post.
        /// </summary>
        private void DeletePost(PlayHubDataSet dataSet, HubMsg_DeletePost msg)
        {
            if (null == dataSet.PostInfo)
            {
                _logger.LogError($"not exist PostInfo.");
                return;
            }

            // PostUid 확인.
            if (false == dataSet.PostInfo.Posts.ContainsKey(msg.PostUid))
            {
                // 우편물 없음.
                SendPacket<HubPacketSC_DeletePost>(dataSet.Pid, HubPacketCode.SC_DeletePost, new HubPacketSC_DeletePost
                {
                    PostUid = msg.PostUid,
                    IsDeleted = false
                });

                _logger.LogError($"not exist post. post uid: {msg.PostUid}");
                return;
            }

            // 수령 확인.
            SendPacket<HubPacketSC_DeletePost>(dataSet.Pid, HubPacketCode.SC_DeletePost, new HubPacketSC_DeletePost
            {
                PostUid = msg.PostUid,
                IsDeleted = true
            });
        }

        /// <summary>
        /// Fail to retrieve post.
        /// </summary>
        private void FailRetrievePost(PlayHubDataSet dataSet, HubMsg_FailRetrievePost msg)
        {
            var pid = dataSet.Pid;
            var postInfo = dataSet.PostInfo;

            // 우편물 검사.
            if (false == postInfo.Posts.ContainsKey(msg.PostUid))
            {
                // 수령 실패
                SendPacket<HubPacketSC_RetrievePost>(pid, HubPacketCode.SC_RetrievePost, new HubPacketSC_RetrievePost
                {
                    IsRetrieved = false
                });

                _logger.LogError($"not exist post. uid: {msg.PostUid}");
                return;
            }

            // 우편물 미수령.
            postInfo.Posts[msg.PostUid].IsRetrived = false;

            // 수령 확인.
            SendPacket<HubPacketSC_FailRetrievePost>(pid, HubPacketCode.SC_FailRetrievePost, new HubPacketSC_FailRetrievePost
            {
                Post = postInfo.Posts[msg.PostUid]
            });
        }
    }
}
