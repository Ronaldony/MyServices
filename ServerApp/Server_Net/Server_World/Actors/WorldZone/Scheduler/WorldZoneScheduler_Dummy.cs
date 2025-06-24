using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Timers;

namespace Server_World.Actors.HuntZone.Scheduler
{
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// HuntZoneScheduler_Enemy.
    /// </summary>
    public sealed class WorldZoneScheduler_Dummy
    {
        private readonly ILogger<WorldZoneScheduler_Dummy> _logger;

        private readonly WorldZoneActorProvider _huntZoneActorProvider;

        // 타이머.
        private System.Timers.Timer _updateDummyTimer;

        public WorldZoneScheduler_Dummy(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldZoneScheduler_Dummy>>();

            _huntZoneActorProvider = serviceProvider.GetRequiredService<WorldZoneActorProvider>();

            Initialize_Timer();
        }

        private void Initialize_Timer()
        {
            // 스폰 타이머 설정.
            _updateDummyTimer = new System.Timers.Timer(TimeSpan.FromMilliseconds(33));
            _updateDummyTimer.Elapsed += OnUpdateDummyElapsed;
            _updateDummyTimer.AutoReset = true;
        }

        /// <summary>
        /// 스폰 타이머 시작.
        /// </summary>
        public void Start()
        {
            _updateDummyTimer.Start();

            _logger.LogInformation("Start HuntZone dummy.");
        }

        /// <summary>
        /// 주기적으로 적 스폰.
        /// </summary>
        private void OnUpdateDummyElapsed(object sender, ElapsedEventArgs e)
        {
            _huntZoneActorProvider.Tell<WorldMsg_UpdateDummy>(new WorldMsg_UpdateDummy());
        }
    }
}
