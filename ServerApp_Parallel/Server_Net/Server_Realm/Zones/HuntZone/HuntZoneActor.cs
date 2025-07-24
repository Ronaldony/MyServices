using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Zones.HuntZone.Router;
    using Server_Realm.Zones.HuntZone.Scheduler;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.GameEvents;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor : ReceiveActor
    {
        private readonly ILogger<HuntZoneActor> _logger;

        // Cancel token.
        private readonly CancellationTokenSource _cts;

        // Router.
        private readonly HuntZoneRouter _huntZoneRouter;

        private readonly HuntZoneScheduler_SpawnEnemy _schdulerSpawnEnemy;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            _cts = new CancellationTokenSource();

            // Router.
            _huntZoneRouter = new HuntZoneRouter(ServiceUtility.GetServiceProvider(), _cts.Token);
            _huntZoneRouter.Initialize();

            // initialize actor receive.
            Initialize_ActorReceive();

            // schedulers.
            _schdulerSpawnEnemy = new HuntZoneScheduler_SpawnEnemy(ServiceUtility.GetServiceProvider());
            _schdulerSpawnEnemy.Start();
        }

        /// <summary>
        /// Stop server.
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
        }

        /// <summary>
        /// Initialize receiver of actor.
        /// </summary>
        private void Initialize_ActorReceive()
        {
            // GameEvent.
            Receive<GameEvent>(gameEvent =>
            {
                if (false == _huntZoneRouter.RouteGameEvent(gameEvent))
                {
                    _logger.LogError($"route game event error. type: {gameEvent.GetGameEventType()}");
                }
            });

            // GameEvent.
            Receive<WorldMsg>(worldMsg =>
            {
                if (false == _huntZoneRouter.RouteWorldMsg(worldMsg))
                {
                    _logger.LogError($"route world message error. type: {worldMsg.MsgType}");
                }
            });
        }
    }
}
