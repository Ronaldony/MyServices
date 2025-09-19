
namespace Server_Control.Hubs.Msg
{
    using Server_Control.Core.Types;

    /// <summary>
    /// HubMsg_ClaimPostFail.
    /// </summary>
    public class HubMsg_ClaimPostFail : HubMsgBase
    {
        public HubMsg_ClaimPostFail()
            : base(Type_HubMsg.ClaimPostFail)
        {
        }

        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        public string PostUid { get; set; }

        /// <summary>
        /// 수령 실패 횟수.
        /// </summary>
        public int FailCount { get; set; }
    }
}
