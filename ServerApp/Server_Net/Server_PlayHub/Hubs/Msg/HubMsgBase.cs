
namespace Server_PlayHub.Hubs.Msg
{
    using Server_PlayHub.Core.Types;

    /// <summary>
    /// HubMsgBase.
    /// </summary>
    public class HubMsgBase
    {
        public HubMsgBase(Type_HubMsg msgType)
        {
            MsgType = msgType;
        }

        /// <summary>
        /// Target pid.
        /// </summary>
        public string TargetPid { get; set; }

        /// <summary>
        /// Msg type.
        /// </summary>
        public Type_HubMsg MsgType { get; set; }
    }
}
