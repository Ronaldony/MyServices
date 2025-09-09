
namespace Server_Control.Hubs.Msg
{
    using Server_Control.Core.Types;

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
