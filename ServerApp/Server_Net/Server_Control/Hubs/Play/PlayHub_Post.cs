using Microsoft.Extensions.Logging;

namespace Server_Control.Hubs.Play
{
    using Framework.Core.Hubs;
    using GameCore;
    using GameCore.HubPacket;
    using PowerArgs;
    using Server_Control.Hubs.Msg;
    using Server_Control.Hubs.Play.Datas;

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
                _logger.LogDebug($"PushPost. uid: {post.PostUid}");
                postInfo.Posts.Add(post.PostUid, post);
            }

            _logger.LogDebug($"PushPost. Zone: {dataSet.ZoneId}");

            // Hub에 진입한 경우, 패킷 전송.
            if (false == string.IsNullOrEmpty(dataSet.ZoneId))
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
        private void ClaimPost(PlayHubDataSet dataSet, HubMsg_ClaimPost msg)
        {
            var pid = dataSet.Pid;

            // PostInfo 로드.
            if (dataSet.PostInfo == null)
            {
                // 수령 실패. PostInfo가 없는 경우 비정상적인 상황. (PlayDomain 접속 시 무조건 로드되기 때문)
                SendPacket<HubPacketSC_ClaimPost>(pid, HubPacketCode.SC_ClaimPost, new HubPacketSC_ClaimPost
                {
                    IsClaimed = false
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
                SendPacket<HubPacketSC_ClaimPost>(pid, HubPacketCode.SC_ClaimPost, new HubPacketSC_ClaimPost
                {
                    IsClaimed = false
                });

                _logger.LogError($"not exist post. uid: {postUid}");
                return;
            }

            // 수령 여부 확인.
            if (postInfo.Posts[postUid].IsClaimed)
            {
                _logger.LogError($"already claim post. uid: {postUid}");
                return;
            }

            // 우편물 수령.
            postInfo.Posts[postUid].IsClaimed = true;

            // 수령 확인.
            SendPacket<HubPacketSC_ClaimPost>(pid, HubPacketCode.SC_ClaimPost, new HubPacketSC_ClaimPost
            {
                IsClaimed = true,
                PostUid = postUid
            });

            // Zone에 아이템 수령 전송.
            _hubActorProvider.Tell<PlayHubMsg_ClaimPost>(new PlayHubMsg_ClaimPost
            {
                Pid = pid,
                Post = postInfo.Posts[postUid],
                ZoneId = dataSet.ZoneId
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

            // 우편물 삭제.
            SendPacket<HubPacketSC_DeletePost>(dataSet.Pid, HubPacketCode.SC_DeletePost, new HubPacketSC_DeletePost
            {
                PostUid = msg.PostUid,
                IsDeleted = true
            });
        }

        /// <summary>
        /// Fail to retrieve post.
        /// </summary>
        private void FailClaimPost(PlayHubDataSet dataSet, HubMsg_FailClaimPost msg)
        {
            var pid = dataSet.Pid;
            var postInfo = dataSet.PostInfo;

            // 우편물 검사.
            if (false == postInfo.Posts.ContainsKey(msg.PostUid))
            {
                // 수령 실패
                SendPacket<HubPacketSC_ClaimPost>(pid, HubPacketCode.SC_ClaimPost, new HubPacketSC_ClaimPost
                {
                    IsClaimed = false
                });

                _logger.LogError($"not exist post. uid: {msg.PostUid}");
                return;
            }

            // 우편물 미수령.
            postInfo.Posts[msg.PostUid].IsClaimed = false;

            // 수령 확인.
            SendPacket<HubPacketSC_FailClaimPost>(pid, HubPacketCode.SC_FailClaimPost, new HubPacketSC_FailClaimPost
            {
                Post = postInfo.Posts[msg.PostUid]
            });
        }
    }
}
