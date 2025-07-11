using Akka.Actor;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone
{
    using GameCore.Interfaces;
    using Server_World.Actors.HuntZone.Router;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Services;
    using Server_World.Core.Utilities;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;
    using Server_World.Server.World;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor : ReceiveActor
    {
        private readonly ILogger<HuntZoneActor> _logger;

        // World group.
        private readonly World_Group _worldGroup;

        // Cancel token.
        private readonly CancellationTokenSource _cts;

        /// <summary>
        /// 필드 적 수.
        /// </summary>
        private int _enemyCount;

        /////////////////////////////////////////////////////
        /// Parallel.
        private readonly HuntZoneRouter _huntZoneRouter;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            _cts = new CancellationTokenSource();

            // Router.
            _huntZoneRouter = new HuntZoneRouter(ServiceUtility.GetServiceProvider(), _cts.Token);

            // initialize actor receive.
            Initialize_ActorReceive();
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
                if (_huntZoneRouter.RouteGameEvent(gameEvent))
                {
                    _logger.LogError($"failed to post game event. type: {gameEvent.GameEventType}");
                }
            });

            // GameEvent.
            Receive<WorldMsg>(worldMsg =>
            {
                if (_huntZoneRouter.RouteWorldMsg(worldMsg))
                {
                    _logger.LogError($"failed to post game event. type: {worldMsg.MsgType}");
                }
            });
        }
    }
}
