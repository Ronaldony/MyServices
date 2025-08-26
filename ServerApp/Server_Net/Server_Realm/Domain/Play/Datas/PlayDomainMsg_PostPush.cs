
namespace Server_Realm.Domain.Play.Datas
{
    using GameCore.Datas.Post;

    /// <summary>
    /// PlayDomainMsg_PostPush.
    /// </summary>
    public class PlayDomainMsg_PostPush : PlayDomainMsg
    {
        public PlayDomainMsg_PostPush()
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
