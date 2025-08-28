
namespace Server_PlayHub.Hubs.Msg
{
    using Server_PlayHub.Core.Types;

    /// <summary>
    /// HubMsg_DeletePost.
    /// </summary>
    public class HubMsg_DeletePost : HubMsgBase
    {
        public HubMsg_DeletePost()
            : base(Type_HubMsg.DeletePost)
        {
        }

        /// <summary>
        /// Post.
        /// key: pid
        /// value: DTO_Post.
        /// </summary>
        public string PostUid { get; set; }
    }
}
