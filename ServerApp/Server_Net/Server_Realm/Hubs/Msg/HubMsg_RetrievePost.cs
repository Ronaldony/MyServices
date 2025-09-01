
namespace Server_Realm.Hubs.Msg
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// HubMsg_RetrievePost.
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
