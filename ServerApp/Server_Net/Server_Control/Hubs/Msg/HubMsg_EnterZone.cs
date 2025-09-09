
namespace Server_Control.Hubs.Msg
{
    using Server_Control.Core.Types;

    /// <summary>
    /// HubMsg_EnterZone.
    /// </summary>
    public class HubMsg_EnterZone : HubMsgBase
    {
        public HubMsg_EnterZone()
            : base(Type_HubMsg.EnterZone)
        {
        }

        /// <summary>
        /// ZoneId.
        /// </summary>
        public string ZoneId { get; set; }
    }
}
