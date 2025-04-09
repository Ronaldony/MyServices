
using Server_World.Core.Worlds;

namespace Server_World.Actors.HuntZone
{
    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 플레이어 아웃.
        /// </summary>
        private void PlayerOut(WorldMsg_PlayerOut msg)
        {
            OutPlayer(msg.Pid);
        }
    }
}
