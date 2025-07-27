using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_Realm.Worlds.MsgTrigger
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.Core.Services;
    using Server_Realm.Units.Enemy;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones;

    /// <summary>
    /// WorldMsgTrigger_SpawnMob.
    /// </summary>
    public sealed class WorldMsgTrigger_SpawnMob
    {
        private readonly ILogger<WorldMsgTrigger_SpawnMob> _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly UnitInitializeService _unitInitializeService;
        private readonly CollisionService _collisionSerivce;

        // Actor provider.
        private readonly IZoneProcessor _zoneProcessor;

        // Configuration.
        private readonly CONF_Area _confArea;
        private readonly CONF_Enemy _confEnemy;

        // Enemy.
        private readonly List<TB_Enemy> _tbMobs;

        // Disposables.
        private readonly CompositeDisposable _subscriptions;

        // Enemy 고유 id.
        private ulong _mobUid;

        public WorldMsgTrigger_SpawnMob(IServiceProvider serviceProvider, IZoneProcessor zoneProcessor)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldMsgTrigger_SpawnMob>>();

            _serviceProvider = serviceProvider;
            _unitInitializeService = serviceProvider.GetRequiredService< UnitInitializeService>();
            _collisionSerivce = serviceProvider.GetRequiredService<CollisionService>();

            _zoneProcessor = zoneProcessor;

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // 데이터 초기화.
            _confArea = tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));
            _confEnemy = tdbService.First<CONF_Enemy>(d => d.Area.Equals(Type_Area.HuntZone));

            _tbMobs = tdbService.SelectAll<TB_Enemy>().ToList();

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

            _logger.LogInformation("start mob spawn.");
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

            _logger.LogInformation("stop mob spawn.");
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

                foreach (var mob in _tbMobs)
                {
                    // 유닛 생성.
                    var dtoMob = _unitInitializeService.CreateEnemy(mob.Id, Type_Area.HuntZone, ++_mobUid);

                    if (null == dtoMob)
                    {
                        _logger.LogError($"fail to create mob. enemyId: {dtoMob.EnemyId}");
                        continue;
                    }

                    // Get section.
                    var section = SectionId.GetSection(dtoMob.Location.Pos);

                    if (false == spawnMsg.MobUnits.ContainsKey(section))
                    {
                        spawnMsg.MobUnits.Add(section, new List<MobUnit>());
                    }

                    spawnMsg.MobUnits[section].Add(new MobUnit(_serviceProvider, dtoMob, _collisionSerivce.Rent(Type_Collision.Enemy)));
                }

                // 메시지 전달.
                _zoneProcessor.SelfTell(spawnMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
