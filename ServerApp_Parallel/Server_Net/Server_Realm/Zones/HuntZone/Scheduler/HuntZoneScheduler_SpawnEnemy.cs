using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_Realm.Zones.HuntZone.Scheduler
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Plays;
    using GameCore.Interfaces;
    using Server_Realm.Core.ActorProviders;
    using Server_Realm.Core.Services;
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
        
        // 지역 정보.
        private readonly CONF_Area _confArea;
        private readonly CONF_Enemy _confEnemy;

        private readonly List<Type_Item> _itemTypes;
        private readonly List<Type_MovePattern> _movePatterns;
        private readonly List<TB_Enemy> _tbEnemies;

        // 임의 섹션 생성기.
        private readonly Random _rand;

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
            _itemTypes = Enum.GetValues<Type_Item>().Where(d => d != Type_Item.None).ToList();
            _movePatterns = Enum.GetValues<Type_MovePattern>().Where(d => d != Type_MovePattern.None).ToList();

            _rand = new Random((int)DateTime.Now.Ticks);

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
                spawnMsg.Enemies = new Dictionary<SectionId, List<DTO_Enemy>>();

                foreach (var enemy in _tbEnemies)
                {
                    // 유닛 생성.
                    var dtoEnemy = _unitInitializeService.CreateEnemy(enemy.Id, Type_Area.HuntZone);

                    if (null == dtoEnemy)
                    {
                        _logger.LogError($"fail to create enemy. enemyId: {dtoEnemy.EnemyId}");
                        continue;
                    }

                    // Enemy uid.
                    dtoEnemy.EnemeyUid = ++_enemyUid;

                    // 이동 정보.
                    var location = dtoEnemy.Location;

                    location.Veltocity = enemy.MoveSpeed;
                    location.MovePattern = _movePatterns.OrderBy(d => new Guid()).First();

                    // Spawn pos.
                    location.SpawnPos = new Vector2(x, y);
                    location.Pos = location.SpawnPos;

                    // Next pos.
                    location.NextPos = location.SpawnPos;

                    // Set direction.
                    location.NextPosDir = new Vector2();

                    _logger.LogDebug($"Spawn enemy. pos: ({x}, {y}), pattern: {location.MovePattern}");

                    // Get section.
                    var section = new SectionId(x / GameData.SECTION_WIDTH, y / GameData.SECTION_HEIGHT);

                    if (false == spawnMsg.Enemies.ContainsKey(section))
                    {
                        spawnMsg.Enemies.Add(section, new List<DTO_Enemy>());
                    }

                    spawnMsg.Enemies[section].Add(dtoEnemy);
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
