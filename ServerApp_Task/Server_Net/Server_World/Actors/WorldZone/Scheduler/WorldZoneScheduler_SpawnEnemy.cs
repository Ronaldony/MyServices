using Akka.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_World.Actors.HuntZone.Scheduler
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.Interfaces;
    using Server_World.Core.ActorProviders;
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// WorldZoneScheduler_SpawnEnemy.
    /// </summary>
    public sealed class WorldZoneScheduler_SpawnEnemy
    {
        private readonly ILogger<WorldZoneScheduler_SpawnEnemy> _logger;

        // Actor provider.
        private readonly WorldZoneActorProvider _huntZoneActorProvider;

        // Schduler.
        private readonly CompositeDisposable _subscriptions;
        
        // 지역 정보.
        private readonly CONF_Area _confArea;
        private readonly CONF_Enemy _confEnemy;

        private readonly List<Type_MovePattern> _movePatterns;
        private readonly List<TB_Enemy> _tbEnemies;

        // 임의 섹션 생성기.
        private readonly Random _rand;

        // Enemy 고유 id.
        private ulong _enemyUid;

        public WorldZoneScheduler_SpawnEnemy(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldZoneScheduler_SpawnEnemy>>();

            _huntZoneActorProvider = serviceProvider.GetRequiredService<WorldZoneActorProvider>();

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // 데이터 초기화.
            _confArea = tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));
            _confEnemy = tdbService.First<CONF_Enemy>(d => d.Area.Equals(Type_Area.HuntZone));

            _tbEnemies = tdbService.SelectAll<TB_Enemy>().ToList();
            _movePatterns = Enum.GetValues<Type_MovePattern>().ToList();

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
                Observable.Timer(TimeSpan.FromSeconds(_confEnemy.SpawnTimeSec))
                .Subscribe(count =>
                {
                    SpawnEnemy();
                }));

            _logger.LogInformation("start world enemy spawn.");
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

            _logger.LogInformation("stop world enemy spawn.");
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
                    //if (_enemyUid > 0)
                    //{
                    //    break;
                    //}

                    // 적 생성.
                    var dtoEnemy = new DTO_Enemy
                    {
                        EnemeyUid = ++_enemyUid,
                        EnemyId = enemy.Id,
                        Health = enemy.Health,
                        DropItem = enemy.DropItem,
                        ActionType = Type_Action.None
                    };

                    // Generate random section.
                    var x = _rand.Next(_confArea.MinX, _confArea.MaxX);
                    var y = _rand.Next(_confArea.MinY, _confArea.MaxY);

                    _logger.LogDebug($"Spawn enemy. pos: ({x}, {y})");

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
