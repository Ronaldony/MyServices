
using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using Server_World.Core.Worlds;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
    {
        /// <summary>
        /// Out player.
        /// </summary>
        private void OutPlayer(WorldMsg_OutPlayer msg)
        {
            _logger.LogWarning($"Log out start. pid: {msg.Pid}");

            if (false == _playerDict.Remove(msg.Pid, out var worldPlayer))
            {
                return;
            }

            var position = worldPlayer.PlayInfo.Position;
            var section = ((int)position.X / GameData.SECTION_WIDTH, (int)position.Y / GameData.SECTION_HEIGHT);

            _sectionPlayerDict[section].TryRemove(msg.Pid, out _);

            // Release.
            _collisionService.Release(worldPlayer.CollisionBox);

            _logger.LogWarning($"Log out end. pid: {msg.Pid}");
        }
    }
}
