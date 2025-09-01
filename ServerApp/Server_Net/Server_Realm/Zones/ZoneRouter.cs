
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Server_Realm.Zones
{
    using Microsoft.Extensions.DependencyInjection;
    using Server_Realm.Entities;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// ZoneRouter.
    /// </summary>
    public sealed class ZoneRouter
    {
        private readonly ILogger<ZoneRouter> _logger;

        /// <summary>
        /// Player units.
        /// key: pid.
        /// value: PlayerUnit.
        /// </summary>
        private readonly ConcurrentDictionary<string, PlayerUnit> PlayerUnits; 

        public ZoneRouter(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ZoneRouter>>();

            PlayerUnits = new ConcurrentDictionary<string, PlayerUnit>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation("ZoneRouter initialize.");
        }

        /// <summary>
        /// Add player.
        /// </summary>
        public bool EnterPlayer(string pid, PlayerUnit pu)
        {
            if (PlayerUnits.ContainsKey(pid))
            {
                return false;
            }

            // Add PlayerUnit.
            PlayerUnits.AddOrUpdate(pid, pu, (key, old) => pu);

            return true;
        }

        /// <summary>
        /// Add player.
        /// </summary>
        public void LeavePlayer(string pid)
        {
            PlayerUnits.Remove(pid, out _);
        }

        /// <summary>
        /// Route to zone.
        /// </summary>
        public bool RouteToZone<T>(string pid, T worldmsg) where T : WorldMsgBase
        {
            if (false == PlayerUnits.TryGetValue(pid, out var pu))
            {
                return false;
            }

            // Send message.
            pu.ZoneProcessor.Tell<T>(worldmsg);

            return true;
        }
    }
}
