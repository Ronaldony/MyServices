
namespace Server_PlayHub.Hubs.Msg
{
    using Server_PlayHub.Core.Types;

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
