using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_World.Actors.HuntZone.Scheduler
{
    using Server_World.Actors.HuntZone.District;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// HuntZoneScheduler_Dummy.
    /// </summary>
    public sealed class HuntZoneScheduler_Dummy
    {
        private readonly ILogger<HuntZoneScheduler_Dummy> _logger;

        // DistrictProcessor.
        private DistrictProcessor _districtProcessor;

        // Scheduler.
        private CompositeDisposable _subscriptions;

        public HuntZoneScheduler_Dummy(IServiceProvider serviceProvider, DistrictProcessor districtProcessor)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneScheduler_Dummy>>();

            _districtProcessor = districtProcessor;

            _subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// 시작.
        /// </summary>
        public void Start()
        {
            // 더미 스케줄링.
            _subscriptions.Add(
                Observable.Timer(TimeSpan.FromMilliseconds(33)) // 33ms
                .Subscribe(repeat =>
                {
                    _districtProcessor.WriteWorldMsg(new WorldMsg_UpdateDummy());
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
