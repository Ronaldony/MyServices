using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_World.Actors.HuntZone.Scheduler
{
    using Framework.Config.ConfigManager;
    using Server_World.Core.ActorProviders;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// WorldServerScheduler_SpawnEnemy.
    /// </summary>
    public sealed class WorldZoneScheduler_UpdateFrame
    {
        private readonly ILogger<WorldZoneScheduler_UpdateFrame> _logger;

        // Actor provider.
        private readonly WorldZoneActorProvider _worldzoneActorProvider;

        // Schduler.
        private readonly CompositeDisposable _subscriptions;

        public WorldZoneScheduler_UpdateFrame(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldZoneScheduler_UpdateFrame>>();

            _worldzoneActorProvider = serviceProvider.GetRequiredService<WorldZoneActorProvider>();

            _subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
            _subscriptions.Add(
                Observable.Timer(TimeSpan.FromSeconds(GameConfigManager._configGame.FrameRate / 1000f))
                .Subscribe(count =>
                {
                    UpdateFrame();
                }));

            _logger.LogInformation("start world update frame.");
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

            _logger.LogInformation("stop world update frame.");
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
                    FrameDeltaTime = GameConfigManager._configGame.FrameRate / 1000f
                };

                _worldzoneActorProvider.Tell<WorldMsg>(updateFrameMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
