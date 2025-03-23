
namespace Server_World.Actors.HuntZone.Message
{
    /// <summary>
    /// HuntZoneMsg_UnloadPlayer.
    /// </summary>
    public record HuntZoneMsg_UnloadPlayer
    {
        /// <summary>
        /// PlayInfo.
        /// </summary>
        public string Pid { get; init; }
    }
}
