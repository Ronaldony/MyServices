
namespace Server_Realm.Domain.Play
{
    using Server_Realm.Domain.Play.Datas;

    /// <summary>
    /// PlayDomain.
    /// </summary>
    public partial class PlayDomain
    {
        /// <summary>
        /// 우편물 푸시.
        /// </summary>
        private async Task PostPushAsync(PlayDomainData domainData, PlayDomainMsg_PostPush msg)
        {
            var pid = domainData.Pid;

            // PostInfo 로드.
            if (domainData.PostInfo == null)
            {
                domainData.PostInfo = await _dbPostInfo.SelectAsync(pid);
            }

            var postInfo = domainData.PostInfo;

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

            // SendPacket.

        }
    }
}
