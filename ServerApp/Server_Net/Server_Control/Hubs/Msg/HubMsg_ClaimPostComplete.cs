
namespace Server_Control.Hubs.Msg
{
    using Server_Control.Core.Types;

    /// <summary>
    /// HubMsg_ClaimPostComplete.
    /// </summary>
    public class HubMsg_ClaimPostComplete : HubMsgBase
    {
        public HubMsg_ClaimPostComplete()
            : base(Type_HubMsg.ClaimPostComplete)
        {
        }

        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        public string PostUid { get; set; }
    }
}
