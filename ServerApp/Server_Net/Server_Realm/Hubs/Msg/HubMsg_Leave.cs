
namespace Server_Realm.Hubs.Msg
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// HubMsg_Leave.
    /// </summary>
    public class HubMsg_Leave : HubMsgBase
    {
        public HubMsg_Leave()
            : base(Type_HubMsg.Leave)
        {

        }
    }
}
