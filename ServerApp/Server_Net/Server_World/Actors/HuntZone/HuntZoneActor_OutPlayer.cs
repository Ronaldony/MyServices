
namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using Server_World.Core.Worlds;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// Out player.
        /// </summary>
        private void OutPlayer(WorldMsg_OutPlayer msg)
        {
            if (false == _playerDict.TryGetValue(msg.Pid, out var worldPlayer))
            {
                _playerDict.Remove(msg.Pid);
                return;
            }

            var pos = worldPlayer.PlayInfo.Position.Pos;
            var section = ((int)pos.X / GameData.SECTION_WIDTH, (int)pos.Y / GameData.SECTION_HEIGHT);

            _sectionPlayerDict[section].TryRemove(msg.Pid, out _);
        }
    }
}
