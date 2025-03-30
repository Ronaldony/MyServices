using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Timers;

namespace Server_World.Actors.HuntZone.Scheduler
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.Interfaces;
    using PowerArgs;
    using Server_World.Core.WorldMsg;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;

    /// <summary>
    /// HuntZoneScheduler_Enemy.
    /// </summary>
    public sealed class HuntZoneScheduler_Enemy
    {
        private readonly ILogger<HuntZoneScheduler_Enemy> _logger;

        private readonly IMemoryPoolService _memoryPoolService;
        private readonly HuntZoneActorProvider _huntZoneActorProvider;

        // 지역 정보.
        private readonly CONF_Area _confArea;
        private readonly CONF_Enemy _confEnemy;

        // 적 정보.
        private readonly IEnumerable<TB_Enemy> _tbEnemies;

        // 적 이동 패턴.
        private List<Type_MovePattern> _movePatterns;

        /// <summary>
        /// 적 이동 패턴 업데이트 현황.
        /// key: Type_MovePattern
        /// value: 업데이트 시간.
        /// </summary>
        private Dictionary<Type_MovePattern, DateTime> _moveUpdateTimeDict;

        // 타이머.
        private readonly System.Timers.Timer _spawnTimer;
        private readonly System.Timers.Timer _updateFrameTimer;

        // 임의 섹션 생성기.
        private readonly Random _randomSection;

        // Enemy 고유 id.
        private ulong _enemyUid;

        public HuntZoneScheduler_Enemy(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneScheduler_Enemy>>();

            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
            _huntZoneActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // 데이터 초기화.
            _confArea = tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));
            _confEnemy = tdbService.First<CONF_Enemy>(d => d.Area.Equals(Type_Area.HuntZone));
            _tbEnemies = tdbService.SelectAll<TB_Enemy>();

            _randomSection = new Random((int)DateTime.Now.Ticks);

            // 스폰 타이머 설정.
            _spawnTimer = new System.Timers.Timer(TimeSpan.FromSeconds(_confEnemy.SpawnTimeSec));
            _spawnTimer.Elapsed += OnSpawnElapsed;
            _spawnTimer.AutoReset = true;

            // 프레임 업데이트 타이머 설정. (기본 30 프레임)
            _updateFrameTimer = new System.Timers.Timer(TimeSpan.FromSeconds(GameConfigManager._configGame.FrameRate / 1000f));
            _updateFrameTimer.Elapsed += OnUpdateFrameElapsed;
            _updateFrameTimer.AutoReset = true;

            // Setup move patterns.
            _movePatterns = Enum.GetValues<Type_MovePattern>().ToList();
            _movePatterns.Remove(Type_MovePattern.None);
            _moveUpdateTimeDict = new Dictionary<Type_MovePattern, DateTime>
            {
                { Type_MovePattern.Pattern1, TimeUtil.Now},
                { Type_MovePattern.Pattern2, TimeUtil.Now},
                { Type_MovePattern.Pattern3, TimeUtil.Now}
            };
        }

        /// <summary>
        /// 스폰 타이머 시작.
        /// </summary>
        public void Start()
        {
            _logger.LogInformation("Start HuntZone enemy spawn.");
            _spawnTimer.Start();
            _updateFrameTimer.Start();
        }

        /// <summary>
        /// 스폰 타이머 종료.
        /// </summary>
        public void Stop()
        {
            _logger.LogInformation("Stop HuntZone enemy spawn.");
            _spawnTimer.Stop();
            _updateFrameTimer.Stop();
        }

        /// <summary>
        /// 주기적으로 적 스폰.
        /// </summary>
        private void OnSpawnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_enemyUid > 0)
            {
                Stop();
                return;
            }

            try
            {
                _logger.LogInformation("Elapse HuntZoneScheduler_Enemy.");

                // Setup pattern.
                var pattern = _movePatterns.OrderBy(d => new Guid()).First();

                // Create spawn enemy message.
                var spawnMsg = new WorldMsg_SpawnEnemy();
                spawnMsg.Enemies = new Dictionary<(int sectionX, int sectionY), List<DTO_Enemy>>();

                //var enemy = _tbEnemies.Last();
                foreach (var enemy in _tbEnemies)
                {
                    // Generate random section.
                    //var x = _randomSection.Next(_huntzoneArea.MinX, _huntzoneArea.MaxX);
                    //var y = _randomSection.Next(_huntzoneArea.MinY, _huntzoneArea.MaxY);
                    var x = 3;
                    var y = 3;

                    // 적 생성.
                    var dtoEnemy = _memoryPoolService.Rent<DTO_Enemy>();

                    dtoEnemy.EnemeyUid = ++_enemyUid;
                    dtoEnemy.EnemyId = enemy.Id;
                    dtoEnemy.Health = enemy.Health;
                    dtoEnemy.DropItem = enemy.DropItem;
                    dtoEnemy.ActionType = Type_Action.None;

                    // 이동 정보.
                    var position = dtoEnemy.Position;

                    position.Veltocity = enemy.MoveSpeed;
                    position.MovePattern = pattern;
                    position.FrameDeltaTime = 0;

                    // 로밍 위치 개수가 3개 이하인 경우 3개 생성.
                    if (dtoEnemy.Position.RoamingPos.Count < 3)
                    {
                        position.RoamingPos.Clear();
                        position.RoamingPos.Add(new Vector2());
                        position.RoamingPos.Add(new Vector2());
                        position.RoamingPos.Add(new Vector2());
                    }   

                    // Spawn pos.
                    var spawnPos = position.Pos;
                    spawnPos.X = x;
                    spawnPos.Y = y;

                    // Next pos.
                    var nextPos = position.NextPos;
                    nextPos.X = spawnPos.X + _randomSection.Next(-6, 6);
                    nextPos.Y = spawnPos.Y + _randomSection.Next(-6, 6);

                    // Roaming position.
                    var firstRoaming = position.RoamingPos[0];
                    firstRoaming.X = spawnPos.X;
                    firstRoaming.Y = spawnPos.Y;

                    var secondRoaming = position.RoamingPos[1];
                    secondRoaming.X = nextPos.X;
                    secondRoaming.Y = nextPos.Y;

                    var thirdRoaming = position.RoamingPos[2];
                    thirdRoaming.X = spawnPos.X + _randomSection.Next(-6, 6);
                    thirdRoaming.Y = spawnPos.Y + _randomSection.Next(-6, 6);

                    // Get section.
                    var section = (x / GameData.SECTION_WIDTH, y / GameData.SECTION_HEIGHT);

                    if (false == spawnMsg.Enemies.ContainsKey(section))
                    {
                        spawnMsg.Enemies.Add(section, new List<DTO_Enemy>());
                    }

                    spawnMsg.Enemies[section].Add(dtoEnemy);
                }

                // 메시지 전달.
                _huntZoneActorProvider.Tell<WorldMsg_SpawnEnemy>(spawnMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// 프레임 업데이트 타이머.
        /// </summary>
        private void OnUpdateFrameElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //_logger.LogInformation("Elapse HuntZoneScheduler_Enemy.");

                // 마지막 이동 패턴 업데이트 시간이 5초 이상 경과한 이동 패턴 가져오기.
                var nowTime = TimeUtil.Now;
                var updateMovePair = _moveUpdateTimeDict.FirstOrDefault(d => TimeUtil.IsPastTime(nowTime, d.Value.AddSeconds(5)));

                // 이동 패턴 업데이트 시간 갱신.
                if (updateMovePair.Key != default)
                {
                    _moveUpdateTimeDict[updateMovePair.Key] = nowTime;
                }

                // UpdateFrame 메시지 생성.
                var updateFrameMsg = new WorldMsg_UpdateFrame();
                updateFrameMsg.MovePattern = updateMovePair.Key;
                updateFrameMsg.FrameDeltaTime = GameConfigManager._configGame.FrameRate / 1000f;

                // 메시지 전달.
                _huntZoneActorProvider.Tell<WorldMsg_UpdateFrame>(updateFrameMsg);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
