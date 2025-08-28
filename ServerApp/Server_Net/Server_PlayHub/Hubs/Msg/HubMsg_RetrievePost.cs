
namespace Server_PlayHub.Hubs.Msg
{
    using GameCore.Datas.Post;
    using Server_PlayHub.Core.Types;

    /// <summary>
    /// HubMsg_PushPost.
    /// </summary>
    public class HubMsg_RetrievePost : HubMsgBase
    {
        public HubMsg_RetrievePost()
            : base(Type_HubMsg.RetrievePost)
        {
        }

        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        public string PostUid { get; set; }
    }
}
