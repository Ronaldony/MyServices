using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_World.Actors.HuntZone.Scheduler
{
    using GameCore;
    using Server_World.Core.ActorProviders;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// WorldServerScheduler_MovePattern.
    /// </summary>
    public sealed class WorldZoneScheduler_MovePattern
    {
        private readonly ILogger<WorldZoneScheduler_MovePattern> _logger;

        // Actor provider.
        private readonly WorldZoneActorProvider _worldZoneActorProvider;

        // Scheduler.
        private readonly CompositeDisposable _subscriptions;

        public WorldZoneScheduler_MovePattern(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldZoneScheduler_MovePattern>>();

            _worldZoneActorProvider = serviceProvider.GetRequiredService<WorldZoneActorProvider>();

            _subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
            // Pattern 1.
            _subscriptions.Add(Observable.Timer(TimeSpan.FromSeconds(3))
                .Subscribe(repeat =>
                {
                    UpdateMovePattern(Type_MovePattern.Pattern1);
                }));

            // Pattern 2.
            _subscriptions.Add(Observable.Timer(TimeSpan.FromSeconds(4))
                .Subscribe(repeat =>
                {
                    UpdateMovePattern(Type_MovePattern.Pattern2);
                }));

            // Pattern 3.
            _subscriptions.Add(Observable.Timer(TimeSpan.FromSeconds(5))
                .Subscribe(repeat =>
                {
                    UpdateMovePattern(Type_MovePattern.Pattern3);
                }));

            _logger.LogInformation("start world move pattern.");
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {
            foreach (var disposable in _subscriptions)
            {
                disposable.Dispose();
            }

            _subscriptions.Clear();

            _logger.LogInformation("stop world move pattern.");
        }

        /// <summary>
        /// 이동 패턴 업데이트.
        /// </summary>
        private void UpdateMovePattern(Type_MovePattern movePattern)
        {
            try
            {
                // 메시지.
                var randomX = Random.Shared.Next(-GameData.MOVEMENT_RANGE_LIMIT, GameData.MOVEMENT_RANGE_LIMIT);
                var randomY = Random.Shared.Next(-GameData.MOVEMENT_RANGE_LIMIT, GameData.MOVEMENT_RANGE_LIMIT);

                _worldZoneActorProvider.Tell<WorldMsg>(
                    new WorldMsg_UpdateMovePattern
                    {
                        MovePattern = movePattern,
                        MovePos = new Vector2(randomX, randomY),
                    });
                }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
