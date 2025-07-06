using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_World.Actors.HuntZone.Scheduler
{
    using Server_World.Core.ActorProviders;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// HuntZoneScheduler_Enemy.
    /// </summary>
    public sealed class WorldZoneScheduler_Dummy
    {
        private readonly ILogger<WorldZoneScheduler_Dummy> _logger;

        // Actor provider.
        private readonly WorldZoneActorProvider _worldZoneActorProvider;

        private CompositeDisposable _subscriptions;

        public WorldZoneScheduler_Dummy(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldZoneScheduler_Dummy>>();

            _worldZoneActorProvider = serviceProvider.GetRequiredService<WorldZoneActorProvider>();

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
                    _worldZoneActorProvider.Tell<WorldMsg>(new WorldMsg_UpdateDummy());
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
