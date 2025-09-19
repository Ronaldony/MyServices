using Microsoft.Extensions.Logging;

namespace Server_Control.Hubs.Play
{
    using Framework.Core.Hubs;
    using GameCore;
    using GameCore.HubPacket;
    using Server_Control.Hubs.Msg;
    using Server_Control.Hubs.Play.Datas;

    /// <summary>
    /// PlayHub.
    /// </summary>
    public partial class PlayHub
    {
        public const int PostMaxTryCount = 3;

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

                postInfo.Posts.Add(post.PostUid, post);

                // 우편물 추가.
                _logger.LogDebug($"PushPost. uid: {post.PostUid}");
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
        private async Task ClaimPostAsync(PlayHubDataSet dataSet, HubMsg_ClaimPost msg)
        {
            var pid = dataSet.Pid;

            // PostInfo 로드.
            if (dataSet.PostInfo == null)
            {
                // 수령 실패. PostInfo가 없는 경우 비정상적인 상황. (PlayDomain 접속 시 무조건 로드되기 때문)
                SendPacket<HubPacketSC_ClaimPostFail>(pid, HubPacketCode.SC_ClaimPostFail, new HubPacketSC_ClaimPostFail
                {
                    PostUid  = msg.PostUid
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
                // 수령 실패. PostInfo가 없는 경우 비정상적인 상황. (PlayDomain 접속 시 무조건 로드되기 때문)
                SendPacket<HubPacketSC_ClaimPostFail>(pid, HubPacketCode.SC_ClaimPostFail, new HubPacketSC_ClaimPostFail
                {
                    PostUid = msg.PostUid
                });

                _logger.LogError($"not exist post. uid: {postUid}");
                return;
            }

            var post = postInfo.Posts[postUid];

            // 이미 수령하였는지 확인.
            if (post.Status == Type_PostStatus.Complete)
            {
                _logger.LogError($"already claim post. uid: {postUid}");
                return;
            }

            // 수령 진행.
            post.Status = Type_PostStatus.Process;

            // Zone에 아이템 수령 전송.
            _hubActorProvider.Tell<ControlMsg_ClaimPost>(new ControlMsg_ClaimPost
            {
                Pid = pid,
                Post = post,
                ZoneId = dataSet.ZoneId,
                TryCount = 1
            });

            // Post 삭제.
            await _dbPost.DeleteAsync(post.PostUid);
        }

        /// <summary>
        /// Fail to retrieve post.
        /// </summary>
        private void ClaimPostFail(PlayHubDataSet dataSet, HubMsg_ClaimPostFail msg)
        {
            var pid = dataSet.Pid;
            var postInfo = dataSet.PostInfo;
            var post = postInfo.Posts[msg.PostUid];

            // 수령 시도 횟수 검사.
            if (msg.FailCount >= PostMaxTryCount)
            {
                // 수령 실패. 우편물 수령 시도가 너무 많은 경우.
                SendPacket<HubPacketSC_ClaimPostFail>(pid, HubPacketCode.SC_ClaimPostFail, new HubPacketSC_ClaimPostFail
                {
                    PostUid = msg.PostUid
                });

                _logger.LogError($"many try claim post eror. pid: {pid} post uid: {msg.PostUid}");
                return;
            }

            // Fail 횟수 증가.
            ++msg.FailCount;

            // Zone에 아이템 수령 전송.
            _hubActorProvider.Tell<ControlMsg_ClaimPost>(new ControlMsg_ClaimPost
            {
                Pid = pid,
                Post = post,
                ZoneId = dataSet.ZoneId,
                TryCount = msg.FailCount
            });
        }

        /// <summary>
        /// Complete to retrieve post.
        /// </summary>
        private void ClaimPostComplete(PlayHubDataSet dataSet, HubMsg_ClaimPostComplete msg)
        {
            var postInfo = dataSet.PostInfo;
            var post = postInfo.Posts[msg.PostUid];

            // 수령 완료.
            post.Status = Type_PostStatus.Complete;
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

    }
}
