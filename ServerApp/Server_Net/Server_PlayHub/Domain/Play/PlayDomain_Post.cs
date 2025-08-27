using Microsoft.Extensions.Logging;

namespace Server_PlayHub.Domain.Play
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_PlayHub.Domain.Play.Datas;
    using Server_PlayHub.Domain.Play.Msg;

    /// <summary>
    /// PlayDomain.
    /// </summary>
    public partial class PlayDomain
    {
        /// <summary>
        /// 우편물 푸시.
        /// </summary>
        private async Task PushPostAsync(PlayDomainDataSet domainDataSet, PlayDomainMsg_PushPost msg)
        {
            var pid = domainDataSet.Pid;

            // PostInfo 로드.
            if (domainDataSet.PostInfo == null)
            {
                domainDataSet.PostInfo = await _dbPostInfo.SelectAsync(pid);
            }

            var postInfo = domainDataSet.PostInfo;

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

            // 새 우편물 푸시.
            SendPacket<DomainPacketSC_PushPost>(pid, DomainPacketCode.SC_PushPost, new DomainPacketSC_PushPost
            {
                PushPosts = msg.Posts[pid]
            });
        }

        /// <summary>
        /// 우편물 수령.
        /// </summary>
        private void RetrievePost(PlayDomainDataSet domainDataSet, PlayDomainMsg_RetrievePost msg)
        {
            var pid = domainDataSet.Pid;

            // PostInfo 로드.
            if (domainDataSet.PostInfo == null)
            {
                // 수령 실패. PostInfo가 없는 경우 비정상적인 상황. (PlayDomain 접속 시 무조건 로드되기 때문)
                SendPacket<DomainPacketSC_RetrievePost>(pid, DomainPacketCode.SC_RetrievePost, new DomainPacketSC_RetrievePost
                {
                    IsRetrieved = false
                });

                _logger.LogError($"load PostInfo error. pid: {pid}");
                return;
            }

            var postInfo = domainDataSet.PostInfo;

            // 우편물 검사.
            if (false == postInfo.Posts.ContainsKey(msg.Post.PostUid))
            {
                // 수령 실패
                SendPacket<DomainPacketSC_RetrievePost>(pid, DomainPacketCode.SC_RetrievePost, new DomainPacketSC_RetrievePost
                {
                    IsRetrieved = false
                });

                _logger.LogError($"not exist post. uid: {msg.Post.PostUid}");
                return;
            }

            // 우편물 수령.
            postInfo.Posts[msg.Post.PostUid].IsRetrived = true;

            // 수령 확인.
            SendPacket<DomainPacketSC_RetrievePost>(pid, DomainPacketCode.SC_RetrievePost, new DomainPacketSC_RetrievePost
            {
                IsRetrieved = true
            });
        }
    }
}
