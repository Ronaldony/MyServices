
namespace Server_Realm.Hubs.Msg
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// HubMsg_Enter.
    /// </summary>
    public class HubMsg_Enter : HubMsgBase
    {
        public HubMsg_Enter()
            : base(Type_HubMsg.Enter)
        {

        }
    }
}
