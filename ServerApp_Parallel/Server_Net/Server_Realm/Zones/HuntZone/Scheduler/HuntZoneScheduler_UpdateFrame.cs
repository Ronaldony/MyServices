using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_Realm.Zones.HuntZone.Scheduler
{
    using Framework.Config.ConfigManager;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones.HuntZone.District;

    /// <summary>
    /// HuntZoneScheduler_UpdateFrame.
    /// </summary>
    public sealed class HuntZoneScheduler_UpdateFrame
    {
        private readonly ILogger<HuntZoneScheduler_UpdateFrame> _logger;

        // DistrictProcessor.
        private readonly DistrictProcessor _districtProcessor;

        // Schduler.
        private readonly CompositeDisposable _subscriptions;

        public HuntZoneScheduler_UpdateFrame(IServiceProvider serviceProvider, DistrictProcessor districtProcessor)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneScheduler_UpdateFrame>>();

            _districtProcessor = districtProcessor;

            _subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
            _subscriptions.Add(
                Observable.Interval(TimeSpan.FromSeconds(GameConfigManager.ConfigGame.FrameRate / 1000f))
                .Subscribe(count =>
                {
                    UpdateFrame();
                }));

            _logger.LogInformation("start huntzone update frame.");
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

            _logger.LogInformation("stop huntzone update frame.");
        }

        /// <summary>
        /// 업데이트 프레임.
        /// </summary>
        private void UpdateFrame()
        {
            try
            {
                // UpdateFrame 메시지 생성.
                var updateFrameMsg = new WorldMsg_UpdateFrame
                {
                    FrameDeltaTime = GameConfigManager.ConfigGame.FrameRate / 1000f
                };

                _districtProcessor.WriteWorldMsg(updateFrameMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
