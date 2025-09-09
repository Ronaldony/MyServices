
namespace Server_Control.Hubs.Msg
{
    using Server_Control.Core.Types;

    /// <summary>
    /// HubMsg_ClaimPost.
    /// </summary>
    public class HubMsg_ClaimPost : HubMsgBase
    {
        public HubMsg_ClaimPost()
            : base(Type_HubMsg.ClaimPost)
        {
        }

        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        public string PostUid { get; set; }
    }
}
