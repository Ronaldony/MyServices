
namespace Server_PlayHub.Hubs.Msg
{
    using GameCore.Datas.Post;
    using Server_PlayHub.Core.Types;

    /// <summary>
    /// HubMsg_PushPost.
    /// </summary>
    public class HubMsg_PushPost : HubMsgBase
    {
        public HubMsg_PushPost()
            : base(Type_HubMsg.PushPost)
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
