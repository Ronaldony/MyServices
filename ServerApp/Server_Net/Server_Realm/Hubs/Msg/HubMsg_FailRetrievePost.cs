
namespace Server_Realm.Hubs.Msg
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// HubMsg_FailRetrievePost.
    /// </summary>
    public class HubMsg_FailRetrievePost : HubMsgBase
    {
        public HubMsg_FailRetrievePost()
            : base(Type_HubMsg.FailRetrievePost)
        {
        }

        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        public string PostUid { get; set; }
    }
}
