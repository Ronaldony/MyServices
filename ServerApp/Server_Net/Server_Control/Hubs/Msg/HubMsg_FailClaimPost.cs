
namespace Server_Control.Hubs.Msg
{
    using Server_Control.Core.Types;

    /// <summary>
    /// HubMsg_FailClaimPost.
    /// </summary>
    public class HubMsg_FailClaimPost : HubMsgBase
    {
        public HubMsg_FailClaimPost()
            : base(Type_HubMsg.FailClaimPost)
        {
        }

        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        public string PostUid { get; set; }
    }
}
