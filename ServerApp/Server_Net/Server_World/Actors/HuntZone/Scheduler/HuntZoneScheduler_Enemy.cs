using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PowerArgs;
using System.Numerics;
using System.Timers;

namespace Server_World.Actors.HuntZone.Scheduler
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.Interfaces;
    using Server_World.Core.Worlds;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Core.Util;

    /// <summary>
    /// HuntZoneScheduler_Enemy.
    /// </summary>
    public sealed class HuntZoneScheduler_Enemy
    {
        private readonly ILogger<HuntZoneScheduler_Enemy> _logger;

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
        private System.Timers.Timer _spawnTimer;
        private System.Timers.Timer _updateFrameTimer;
        private System.Timers.Timer _updateMovePattern;

        // 임의 섹션 생성기.
        private readonly Random _randomSection;

        // Enemy 고유 id.
        private ulong _enemyUid;

        public HuntZoneScheduler_Enemy(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneScheduler_Enemy>>();

            _huntZoneActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // 데이터 초기화.
            _confArea = tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));
            _confEnemy = tdbService.First<CONF_Enemy>(d => d.Area.Equals(Type_Area.HuntZone));
            _tbEnemies = tdbService.SelectAll<TB_Enemy>();

            _randomSection = new Random((int)DateTime.Now.Ticks);

            Initialize_Timer();

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

        private void Initialize_Timer()
        {
            // 스폰 타이머 설정.
            _spawnTimer = new System.Timers.Timer(TimeSpan.FromSeconds(_confEnemy.SpawnTimeSec));
            _spawnTimer.Elapsed += OnSpawnElapsed;
            _spawnTimer.AutoReset = true;

            // 프레임 업데이트 타이머 설정. (기본 30 프레임)
            _updateFrameTimer = new System.Timers.Timer(TimeSpan.FromSeconds(GameConfigManager._configGame.FrameRate / 1000f));
            _updateFrameTimer.Elapsed += OnUpdateFrameElapsed;
            _updateFrameTimer.AutoReset = true;

            // 이동 패턴 변경 타이머 설정.
            _updateMovePattern = new System.Timers.Timer(TimeSpan.FromSeconds(1));
            _updateMovePattern.Elapsed += OnUpdateMovePattern;
            _updateMovePattern.AutoReset = true;
        }

        /// <summary>
        /// 스폰 타이머 시작.
        /// </summary>
        public void Start()
        {
            _spawnTimer.Start();
            _updateFrameTimer.Start();
            _updateMovePattern.Start();

            _logger.LogInformation("Start HuntZone enemy spawn.");
        }

        /// <summary>
        /// 스폰 타이머 종료.
        /// </summary>
        public void Stop()
        {
            _spawnTimer.Stop();
            _updateFrameTimer.Stop();
            _updateMovePattern.Stop();

            _logger.LogInformation("Stop HuntZone enemy spawn.");
        }

        /// <summary>
        /// Start spawn.
        /// </summary>
        public void StartSpawn()
        {
            _spawnTimer.Start();
        }

        /// <summary>
        /// Stop spawn.
        /// </summary>
        public void StopSpawn()
        {
            _spawnTimer.Stop();
        }

        /// <summary>
        /// 주기적으로 적 스폰.
        /// </summary>
        private void OnSpawnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _logger.LogInformation("Elapse HuntZoneScheduler_Enemy.");

                // Setup pattern.
                var pattern = _movePatterns.OrderBy(d => new Guid()).First();

                // Create spawn enemy message.
                var spawnMsg = new WorldMsg_SpawnEnemy();
                spawnMsg.Enemies = new Dictionary<(int sectionX, int sectionY), List<DTO_Enemy>>();

                //var enemy = _tbEnemies.First();
                foreach (var enemy in _tbEnemies)
                {
                    // 적 생성.
                    var dtoEnemy = new DTO_Enemy();

                    dtoEnemy.EnemeyUid = ++_enemyUid;
                    dtoEnemy.EnemyId = enemy.Id;
                    dtoEnemy.Health = enemy.Health;
                    dtoEnemy.DropItem = enemy.DropItem;
                    dtoEnemy.ActionType = Type_Action.None;

                    // 이동 정보.
                    var position = dtoEnemy.Position;

                    position.Veltocity = enemy.MoveSpeed;
                    position.MovePattern = pattern;

                    // Generate random section.
                    var x = _randomSection.Next(_confArea.MinX, _confArea.MaxX);
                    var y = _randomSection.Next(_confArea.MinY, _confArea.MaxY);

                    _logger.LogDebug($"Spawn enemy. pos: ({x}, {y})");

                    // Spawn pos.
                    position.Pos = new Vector2(x, y);

                    // Next pos.
                    position.NextPos = new Vector2(position.Pos.X + _randomSection.Next(-3, 3), position.Pos.Y + _randomSection.Next(-3, 3));

                    // Set direction.
                    position.NextPosDir = Vector2.Normalize(position.NextPos - position.Pos);

                    // 로밍 위치 개수가 3개 이하인 경우 3개 생성.
                    position.RoamingPos.Add(position.Pos);
                    position.RoamingPos.Add(position.NextPos);
                    position.RoamingPos.Add(new Vector2(position.Pos.X + _randomSection.Next(-3, 3), position.Pos.Y + _randomSection.Next(-3, 3)));

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
                // UpdateFrame 메시지 생성.
                var updateFrameMsg = new WorldMsg_UpdateFrame();

                updateFrameMsg.FrameDeltaTime = GameConfigManager._configGame.FrameRate / 1000f;

                _huntZoneActorProvider.Tell<WorldMsg_UpdateFrame>(updateFrameMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// 이동 패턴 업데이트 타이머.
        /// </summary>
        private void OnUpdateMovePattern(object sender, ElapsedEventArgs e)
        {
            try
            {
                // 마지막 이동 패턴 업데이트 시간이 5초 이상 경과한 이동 패턴 가져오기.
                var nowTime = TimeUtil.Now;
                var updateMovePair = _moveUpdateTimeDict.FirstOrDefault(d => TimeUtil.IsPastTime(d.Value.AddSeconds(5), nowTime));

                // 이동 패턴 업데이트 시간 갱신.
                if (updateMovePair.Key == default)
                {
                    return;
                }

                _moveUpdateTimeDict[updateMovePair.Key] = nowTime;

                // 메시지.
                var msg = new WorldMsg_UpdateMovePattern
                {
                    MovePattern = updateMovePair.Key
                };

                //_huntZoneActorProvider.Tell<WorldMsg_UpdateMovePattern>(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
