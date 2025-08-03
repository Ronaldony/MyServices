using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_Realm.Worlds.MsgTrigger
{
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones;

    /// <summary>
    /// WorldMsgTrigger_Dummy.
    /// </summary>
    public sealed class WorldMsgTrigger_Dummy
    {
        private readonly ILogger<WorldMsgTrigger_Dummy> _logger;

        // DistrictProcessor.
        private IZoneProcessor _zoneProcessor;

        // Scheduler.
        private CompositeDisposable _subscriptions;

        public WorldMsgTrigger_Dummy(IServiceProvider serviceProvider, IZoneProcessor zoneProcessor)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldMsgTrigger_Dummy>>();

            _zoneProcessor = zoneProcessor;

            _subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// 시작.
        /// </summary>
        public void Start()
        {
            // 더미 스케줄링.
            _subscriptions.Add(
                Observable.Interval(TimeSpan.FromMilliseconds(33)) // 33ms
                .Subscribe(repeat =>
                {
                    _zoneProcessor.Tell(new WorldMsg_UpdateDummy());
                }));

            _logger.LogInformation("start world dummy.");
        }

        /// <summary>
        /// 정지.
        /// </summary>
        public void Stop()
        {
            foreach (var disposable in _subscriptions)
            {
                disposable.Dispose();
            }

            _subscriptions.Clear();

            _logger.LogInformation("stop world dummy.");
        }
    }
}
