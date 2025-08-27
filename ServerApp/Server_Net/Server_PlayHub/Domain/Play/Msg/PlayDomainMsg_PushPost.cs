namespace Server_PlayHub.Domain.Play.Msg
{
    using GameCore.Datas.Post;

    /// <summary>
    /// PlayDomainMsg_PushPost.
    /// </summary>
    public class PlayDomainMsg_PushPost : PlayDomainMsg
    {
        public PlayDomainMsg_PushPost()
        {
            Posts = new Dictionary<string, List<DTO_Post>>();
        }

        /// <summary>
        /// Post.
        /// key: pid
        /// value: DTO_Post.
        /// </summary>
        public Dictionary<string, List<DTO_Post>> Posts { get; set; }
    }
}
