using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_Realm.Zones.HuntZone.Scheduler
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.Core.ActorProviders;
    using Server_Realm.Core.Services;
    using Server_Realm.Units.Enemy;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// HuntZoneScheduler_SpawnEnemy.
    /// </summary>
    public sealed class HuntZoneScheduler_SpawnEnemy
    {
        private readonly ILogger<HuntZoneScheduler_SpawnEnemy> _logger;

        // Actor provider.
        private readonly HuntZoneActorProvider _huntZoneActorProvider;

        private readonly UnitInitializeService _unitInitializeService;

        // Schduler.
        private readonly CompositeDisposable _subscriptions;
        
        // Configuration.
        private readonly CONF_Area _confArea;
        private readonly CONF_Enemy _confEnemy;

        // Enemy.
        private readonly List<TB_Enemy> _tbEnemies;

        // Enemy 고유 id.
        private ulong _enemyUid;

        public HuntZoneScheduler_SpawnEnemy(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneScheduler_SpawnEnemy>>();

            _huntZoneActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();

            _unitInitializeService = serviceProvider.GetRequiredService< UnitInitializeService>();

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // 데이터 초기화.
            _confArea = tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));
            _confEnemy = tdbService.First<CONF_Enemy>(d => d.Area.Equals(Type_Area.HuntZone));

            _tbEnemies = tdbService.SelectAll<TB_Enemy>().ToList();

            _subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// 스폰 타이머 시작.
        /// </summary>
        public void Start()
        {
            // 구독.
            _subscriptions.Add(
                Observable.Interval(TimeSpan.FromSeconds(_confEnemy.SpawnTimeSec))
                .Subscribe(count =>
                {
                    SpawnEnemy();
                }));

            _logger.LogInformation("start huntzone enemy spawn.");
        }

        /// <summary>
        /// 스폰 타이머 종료.
        /// </summary>
        public void Stop()
        {
            foreach (var disposable in _subscriptions)
            {
                disposable.Dispose();
            }

            _subscriptions.Clear();

            _logger.LogInformation("stop huntzone enemy spawn.");
        }

        /// <summary>
        /// 주기적으로 적 스폰.
        /// </summary>
        private void SpawnEnemy()
        {
            try
            {
                // Create spawn enemy message.
                var spawnMsg = new WorldMsg_SpawnEnemy();

                foreach (var enemy in _tbEnemies)
                {
                    // 유닛 생성.
                    var dtoEnemy = _unitInitializeService.CreateEnemy(enemy.Id, Type_Area.HuntZone, ++_enemyUid);

                    if (null == dtoEnemy)
                    {
                        _logger.LogError($"fail to create enemy. enemyId: {dtoEnemy.EnemyId}");
                        continue;
                    }

                    // Get section.
                    var section = SectionId.GetSection(dtoEnemy.Location.Pos);

                    if (false == spawnMsg.Enemies.ContainsKey(section))
                    {
                        spawnMsg.Enemies.Add(section, new List<EnemyUnit>());
                    }

                    spawnMsg.Enemies[section].Add(new EnemyUnit
                    {
                        Enemy = dtoEnemy,
                    });
                }

                // 메시지 전달.
                _huntZoneActorProvider.Tell<WorldMsg>(spawnMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
