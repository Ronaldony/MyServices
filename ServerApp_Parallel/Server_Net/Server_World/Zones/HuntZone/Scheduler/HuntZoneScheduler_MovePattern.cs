using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_World.Zones.HuntZone.Scheduler
{
    using GameCore;
    using Server_World.Zones.HuntZone.District;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// WorldServerScheduler_MovePattern.
    /// </summary>
    public sealed class HuntZoneScheduler_MovePattern
    {
        private readonly ILogger<HuntZoneScheduler_MovePattern> _logger;

        // DistrictProcessor.
        private DistrictProcessor _districtProcessor;

        // Scheduler.
        private readonly CompositeDisposable _subscriptions;

        public HuntZoneScheduler_MovePattern(IServiceProvider serviceProvider, DistrictProcessor districtProcessor)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneScheduler_MovePattern>>();

            _districtProcessor = districtProcessor;

            _subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
            var rand = new Random((int)TimeUtil.Now.Ticks);

            // Generate move pattern.
            foreach (var movePattern in Enum.GetValues<Type_MovePattern>().Where(d => d == Type_MovePattern.None))
            {
                _subscriptions.Add(Observable.Interval(TimeSpan.FromSeconds(rand.Next(3, 6)))
                .Subscribe(repeat =>
                {
                    UpdateMovePattern(movePattern);
                }));
            }

            _logger.LogInformation("start huntzone move pattern.");
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

            _logger.LogInformation("stop huntzone move pattern.");
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

                _districtProcessor.WriteWorldMsg(new WorldMsg_UpdateMovePattern
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
