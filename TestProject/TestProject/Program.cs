using MySqlConnector;
using System.Collections.Concurrent;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

// ===== 1. 핵심 도메인 모델 =====

public readonly record struct Vector3(float X, float Y, float Z)
{
    public static Vector3 Zero => new(0, 0, 0);
    public float DistanceTo(Vector3 other) =>
        MathF.Sqrt(MathF.Pow(X - other.X, 2) + MathF.Pow(Y - other.Y, 2) + MathF.Pow(Z - other.Z, 2));
    public Vector3 Add(Vector3 other) => new(X + other.X, Y + other.Y, Z + other.Z);
    public Vector3 Multiply(float scalar) => new(X * scalar, Y * scalar, Z * scalar);
    public Vector3 Normalize()
    {
        var length = MathF.Sqrt(X * X + Y * Y + Z * Z);
        return length > 0 ? new Vector3(X / length, Y / length, Z / length) : Zero;
    }
}

public readonly record struct SectionId(int X, int Y)
{
    public IEnumerable<SectionId> GetNeighbors(int radius = 1)
    {
        for (int x = X - radius; x <= X + radius; x++)
        {
            for (int y = Y - radius; y <= Y + radius; y++)
            {
                yield return new SectionId(x, y);
            }
        }
    }
}

// 플레이어 상태
public record Player(
    int PlayerId,
    string Name,
    Vector3 Position,
    Vector3 Velocity,
    SectionId Section,
    int Health,
    int MaxHealth,
    int Mana,
    int MaxMana,
    int Level,
    float CollisionRadius,
    DateTime LastUpdate,
    bool IsActive = true
)
{
    public bool IsAlive => Health > 0 && IsActive;
    public float HealthPercent => MaxHealth > 0 ? (float)Health / MaxHealth : 0;
}

// 적/몬스터 상태
public record Enemy(
    int EnemyId,
    string Type,
    Vector3 Position,
    Vector3 Velocity,
    SectionId Section,
    int Health,
    int MaxHealth,
    int Level,
    float CollisionRadius,
    Vector3 SpawnPoint,
    float DetectionRange,
    float AttackRange,
    int Damage,
    EnemyState State,
    int? TargetPlayerId,
    DateTime LastUpdate,
    bool IsActive = true
)
{
    public bool IsAlive => Health > 0 && IsActive;
    public bool CanAttack => State == EnemyState.Attacking && TargetPlayerId.HasValue;
}

public enum EnemyState
{
    Idle,
    Patrolling,
    Chasing,
    Attacking,
    Retreating,
    Dead
}

// 게임 이벤트들
public abstract record GameEvent(DateTime Timestamp, SectionId Section);
public record PlayerSpawnEvent(DateTime Timestamp, SectionId Section, Player Player) : GameEvent(Timestamp, Section);
public record EnemySpawnEvent(DateTime Timestamp, SectionId Section, Enemy Enemy) : GameEvent(Timestamp, Section);
public record PlayerMoveEvent(DateTime Timestamp, SectionId Section, int PlayerId, Vector3 From, Vector3 To) : GameEvent(Timestamp, Section);
public record EnemyMoveEvent(DateTime Timestamp, SectionId Section, int EnemyId, Vector3 From, Vector3 To) : GameEvent(Timestamp, Section);
public record CollisionEvent(DateTime Timestamp, SectionId Section, int Entity1Id, int Entity2Id, EntityType Entity1Type, EntityType Entity2Type, Vector3 Point) : GameEvent(Timestamp, Section);
public record CombatEvent(DateTime Timestamp, SectionId Section, int AttackerId, int TargetId, EntityType AttackerType, EntityType TargetType, int Damage) : GameEvent(Timestamp, Section);
public record PlayerDeathEvent(DateTime Timestamp, SectionId Section, int PlayerId, int? KillerId, EntityType? KillerType) : GameEvent(Timestamp, Section);
public record EnemyDeathEvent(DateTime Timestamp, SectionId Section, int EnemyId, int? KillerId) : GameEvent(Timestamp, Section);

public enum EntityType
{
    Player,
    Enemy,
    Object
}

// 네트워크 메시지들
public abstract record NetworkMessage(int ClientId, MessageType Type, DateTime Timestamp);
public record PlayerJoinRequest(int ClientId, DateTime Timestamp, string PlayerName) : NetworkMessage(ClientId, MessageType.PlayerJoin, Timestamp);
public record PlayerMoveRequest(int ClientId, DateTime Timestamp, Vector3 Position, Vector3 Velocity) : NetworkMessage(ClientId, MessageType.PlayerMove, Timestamp);
public record PlayerAttackRequest(int ClientId, DateTime Timestamp, int TargetId, EntityType TargetType) : NetworkMessage(ClientId, MessageType.PlayerAttack, Timestamp);
public record GameStateUpdate(int ClientId, DateTime Timestamp, Player[] Players, Enemy[] Enemies) : NetworkMessage(ClientId, MessageType.GameState, Timestamp);
public record CombatUpdate(int ClientId, DateTime Timestamp, CombatEvent Combat) : NetworkMessage(ClientId, MessageType.Combat, Timestamp);

public enum MessageType
{
    PlayerJoin,
    PlayerMove,
    PlayerAttack,
    GameState,
    Combat,
    Error
}

// ===== 2. MySQL 데이터베이스 레이어 =====

public interface IGameRepository
{
    Task<Player?> GetPlayerAsync(int playerId);
    Task<Player> CreatePlayerAsync(string name, Vector3 spawnPosition);
    Task UpdatePlayerAsync(Player player);
    Task<Enemy[]> GetEnemiesInSectionAsync(SectionId section);
    Task<Enemy> CreateEnemyAsync(string type, Vector3 position, SectionId section);
    Task UpdateEnemyAsync(Enemy enemy);
    Task LogCombatEventAsync(CombatEvent combatEvent);
    Task<Vector3[]> GetSpawnPointsAsync(string type);
}

public class GameRepository : IGameRepository
{
    private readonly string _connectionString;
    private readonly ActionBlock<Func<Task>> _dbWriteQueue;

    public GameRepository(string connectionString)
    {
        _connectionString = connectionString;

        // 데이터베이스 쓰기 작업 직렬화
        _dbWriteQueue = new ActionBlock<Func<Task>>(
            async writeTask => await writeTask(),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1, // 쓰기 작업 순서 보장
                BoundedCapacity = 10000
            });
    }

    public async Task<Player?> GetPlayerAsync(int playerId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT PlayerId, Name, PosX, PosY, PosZ, VelX, VelY, VelZ, 
                   SectionX, SectionY, Health, MaxHealth, Mana, MaxMana, Level, LastUpdate
            FROM Players 
            WHERE PlayerId = @playerId AND IsActive = 1";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@playerId", playerId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Player(
                PlayerId: reader.GetInt32("PlayerId"),
                Name: reader.GetString("Name"),
                Position: new Vector3(reader.GetFloat("PosX"), reader.GetFloat("PosY"), reader.GetFloat("PosZ")),
                Velocity: new Vector3(reader.GetFloat("VelX"), reader.GetFloat("VelY"), reader.GetFloat("VelZ")),
                Section: new SectionId(reader.GetInt32("SectionX"), reader.GetInt32("SectionY")),
                Health: reader.GetInt32("Health"),
                MaxHealth: reader.GetInt32("MaxHealth"),
                Mana: reader.GetInt32("Mana"),
                MaxMana: reader.GetInt32("MaxMana"),
                Level: reader.GetInt32("Level"),
                CollisionRadius: 1.0f,
                LastUpdate: reader.GetDateTime("LastUpdate")
            );
        }

        return null;
    }

    public async Task<Player> CreatePlayerAsync(string name, Vector3 spawnPosition)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO Players (Name, PosX, PosY, PosZ, VelX, VelY, VelZ, 
                               SectionX, SectionY, Health, MaxHealth, Mana, MaxMana, Level, LastUpdate, IsActive)
            VALUES (@name, @posX, @posY, @posZ, 0, 0, 0, @sectionX, @sectionY, 100, 100, 50, 50, 1, @lastUpdate, 1);
            SELECT LAST_INSERT_ID();";

        var section = CalculateSection(spawnPosition);

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@posX", spawnPosition.X);
        command.Parameters.AddWithValue("@posY", spawnPosition.Y);
        command.Parameters.AddWithValue("@posZ", spawnPosition.Z);
        command.Parameters.AddWithValue("@sectionX", section.X);
        command.Parameters.AddWithValue("@sectionY", section.Y);
        command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);

        var playerId = Convert.ToInt32(await command.ExecuteScalarAsync());

        return new Player(
            PlayerId: playerId,
            Name: name,
            Position: spawnPosition,
            Velocity: Vector3.Zero,
            Section: section,
            Health: 100,
            MaxHealth: 100,
            Mana: 50,
            MaxMana: 50,
            Level: 1,
            CollisionRadius: 1.0f,
            LastUpdate: DateTime.UtcNow
        );
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        // 비동기 쓰기 큐에 추가
        await _dbWriteQueue.SendAsync(async () =>
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE Players 
                SET PosX = @posX, PosY = @posY, PosZ = @posZ,
                    VelX = @velX, VelY = @velY, VelZ = @velZ,
                    SectionX = @sectionX, SectionY = @sectionY,
                    Health = @health, Mana = @mana, LastUpdate = @lastUpdate
                WHERE PlayerId = @playerId";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@playerId", player.PlayerId);
            command.Parameters.AddWithValue("@posX", player.Position.X);
            command.Parameters.AddWithValue("@posY", player.Position.Y);
            command.Parameters.AddWithValue("@posZ", player.Position.Z);
            command.Parameters.AddWithValue("@velX", player.Velocity.X);
            command.Parameters.AddWithValue("@velY", player.Velocity.Y);
            command.Parameters.AddWithValue("@velZ", player.Velocity.Z);
            command.Parameters.AddWithValue("@sectionX", player.Section.X);
            command.Parameters.AddWithValue("@sectionY", player.Section.Y);
            command.Parameters.AddWithValue("@health", player.Health);
            command.Parameters.AddWithValue("@mana", player.Mana);
            command.Parameters.AddWithValue("@lastUpdate", player.LastUpdate);

            await command.ExecuteNonQueryAsync();
        });
    }

    public async Task<Enemy[]> GetEnemiesInSectionAsync(SectionId section)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT EnemyId, Type, PosX, PosY, PosZ, VelX, VelY, VelZ,
                   SectionX, SectionY, Health, MaxHealth, Level, SpawnX, SpawnY, SpawnZ,
                   DetectionRange, AttackRange, Damage, State, TargetPlayerId, LastUpdate
            FROM Enemies 
            WHERE SectionX BETWEEN @minX AND @maxX 
              AND SectionY BETWEEN @minY AND @maxY 
              AND IsActive = 1";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@minX", section.X - 1);
        command.Parameters.AddWithValue("@maxX", section.X + 1);
        command.Parameters.AddWithValue("@minY", section.Y - 1);
        command.Parameters.AddWithValue("@maxY", section.Y + 1);

        var enemies = new List<Enemy>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            enemies.Add(new Enemy(
                EnemyId: reader.GetInt32("EnemyId"),
                Type: reader.GetString("Type"),
                Position: new Vector3(reader.GetFloat("PosX"), reader.GetFloat("PosY"), reader.GetFloat("PosZ")),
                Velocity: new Vector3(reader.GetFloat("VelX"), reader.GetFloat("VelY"), reader.GetFloat("VelZ")),
                Section: new SectionId(reader.GetInt32("SectionX"), reader.GetInt32("SectionY")),
                Health: reader.GetInt32("Health"),
                MaxHealth: reader.GetInt32("MaxHealth"),
                Level: reader.GetInt32("Level"),
                CollisionRadius: 1.5f,
                SpawnPoint: new Vector3(reader.GetFloat("SpawnX"), reader.GetFloat("SpawnY"), reader.GetFloat("SpawnZ")),
                DetectionRange: reader.GetFloat("DetectionRange"),
                AttackRange: reader.GetFloat("AttackRange"),
                Damage: reader.GetInt32("Damage"),
                State: (EnemyState)reader.GetInt32("State"),
                TargetPlayerId: reader.IsDBNull("TargetPlayerId") ? null : reader.GetInt32("TargetPlayerId"),
                LastUpdate: reader.GetDateTime("LastUpdate")
            ));
        }

        return enemies.ToArray();
    }

    public async Task<Enemy> CreateEnemyAsync(string type, Vector3 position, SectionId section)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO Enemies (Type, PosX, PosY, PosZ, VelX, VelY, VelZ,
                               SectionX, SectionY, Health, MaxHealth, Level, SpawnX, SpawnY, SpawnZ,
                               DetectionRange, AttackRange, Damage, State, LastUpdate, IsActive)
            VALUES (@type, @posX, @posY, @posZ, 0, 0, 0, @sectionX, @sectionY, 
                   @health, @health, @level, @posX, @posY, @posZ, @detectionRange, @attackRange, @damage, 0, @lastUpdate, 1);
            SELECT LAST_INSERT_ID();";

        var (health, level, detectionRange, attackRange, damage) = GetEnemyStats(type);

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@type", type);
        command.Parameters.AddWithValue("@posX", position.X);
        command.Parameters.AddWithValue("@posY", position.Y);
        command.Parameters.AddWithValue("@posZ", position.Z);
        command.Parameters.AddWithValue("@sectionX", section.X);
        command.Parameters.AddWithValue("@sectionY", section.Y);
        command.Parameters.AddWithValue("@health", health);
        command.Parameters.AddWithValue("@level", level);
        command.Parameters.AddWithValue("@detectionRange", detectionRange);
        command.Parameters.AddWithValue("@attackRange", attackRange);
        command.Parameters.AddWithValue("@damage", damage);
        command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);

        var enemyId = Convert.ToInt32(await command.ExecuteScalarAsync());

        return new Enemy(
            EnemyId: enemyId,
            Type: type,
            Position: position,
            Velocity: Vector3.Zero,
            Section: section,
            Health: health,
            MaxHealth: health,
            Level: level,
            CollisionRadius: 1.5f,
            SpawnPoint: position,
            DetectionRange: detectionRange,
            AttackRange: attackRange,
            Damage: damage,
            State: EnemyState.Idle,
            TargetPlayerId: null,
            LastUpdate: DateTime.UtcNow
        );
    }

    public async Task UpdateEnemyAsync(Enemy enemy)
    {
        await _dbWriteQueue.SendAsync(async () =>
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE Enemies 
                SET PosX = @posX, PosY = @posY, PosZ = @posZ,
                    VelX = @velX, VelY = @velY, VelZ = @velZ,
                    SectionX = @sectionX, SectionY = @sectionY,
                    Health = @health, State = @state, TargetPlayerId = @targetPlayerId, LastUpdate = @lastUpdate
                WHERE EnemyId = @enemyId";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@enemyId", enemy.EnemyId);
            command.Parameters.AddWithValue("@posX", enemy.Position.X);
            command.Parameters.AddWithValue("@posY", enemy.Position.Y);
            command.Parameters.AddWithValue("@posZ", enemy.Position.Z);
            command.Parameters.AddWithValue("@velX", enemy.Velocity.X);
            command.Parameters.AddWithValue("@velY", enemy.Velocity.Y);
            command.Parameters.AddWithValue("@velZ", enemy.Velocity.Z);
            command.Parameters.AddWithValue("@sectionX", enemy.Section.X);
            command.Parameters.AddWithValue("@sectionY", enemy.Section.Y);
            command.Parameters.AddWithValue("@health", enemy.Health);
            command.Parameters.AddWithValue("@state", (int)enemy.State);
            command.Parameters.AddWithValue("@targetPlayerId", enemy.TargetPlayerId);
            command.Parameters.AddWithValue("@lastUpdate", enemy.LastUpdate);

            await command.ExecuteNonQueryAsync();
        });
    }

    public async Task LogCombatEventAsync(CombatEvent combatEvent)
    {
        await _dbWriteQueue.SendAsync(async () =>
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO CombatLog (AttackerId, TargetId, AttackerType, TargetType, Damage, Timestamp)
                VALUES (@attackerId, @targetId, @attackerType, @targetType, @damage, @timestamp)";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@attackerId", combatEvent.AttackerId);
            command.Parameters.AddWithValue("@targetId", combatEvent.TargetId);
            command.Parameters.AddWithValue("@attackerType", (int)combatEvent.AttackerType);
            command.Parameters.AddWithValue("@targetType", (int)combatEvent.TargetType);
            command.Parameters.AddWithValue("@damage", combatEvent.Damage);
            command.Parameters.AddWithValue("@timestamp", combatEvent.Timestamp);

            await command.ExecuteNonQueryAsync();
        });
    }

    public async Task<Vector3[]> GetSpawnPointsAsync(string type)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT PosX, PosY, PosZ FROM SpawnPoints WHERE Type = @type AND IsActive = 1";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@type", type);

        var spawnPoints = new List<Vector3>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            spawnPoints.Add(new Vector3(
                reader.GetFloat("PosX"),
                reader.GetFloat("PosY"),
                reader.GetFloat("PosZ")
            ));
        }

        return spawnPoints.ToArray();
    }

    private static SectionId CalculateSection(Vector3 position, float sectionSize = 100.0f)
    {
        return new SectionId(
            (int)Math.Floor(position.X / sectionSize),
            (int)Math.Floor(position.Z / sectionSize)
        );
    }

    private static (int health, int level, float detectionRange, float attackRange, int damage) GetEnemyStats(string type)
    {
        return type.ToLower() switch
        {
            "goblin" => (50, 1, 10.0f, 2.0f, 15),
            "orc" => (100, 3, 15.0f, 3.0f, 25),
            "dragon" => (500, 10, 30.0f, 5.0f, 100),
            _ => (30, 1, 8.0f, 1.5f, 10)
        };
    }
}

// ===== 3. 공간 분할 시스템 =====

public class SpatialManager
{
    private readonly ConcurrentDictionary<SectionId, ConcurrentHashSet<int>> _playerSections = new();
    private readonly ConcurrentDictionary<SectionId, ConcurrentHashSet<int>> _enemySections = new();
    private readonly ConcurrentDictionary<int, SectionId> _playerPositions = new();
    private readonly ConcurrentDictionary<int, SectionId> _enemyPositions = new();
    private readonly float _sectionSize;

    public SpatialManager(float sectionSize = 100.0f)
    {
        _sectionSize = sectionSize;
    }

    public SectionId GetSection(Vector3 position) => new(
        (int)Math.Floor(position.X / _sectionSize),
        (int)Math.Floor(position.Z / _sectionSize)
    );

    public async Task<bool> MovePlayerToSectionAsync(int playerId, SectionId newSection)
    {
        if (_playerPositions.TryGetValue(playerId, out var oldSection))
        {
            if (oldSection == newSection) return false;

            // 이전 섹션에서 제거
            if (_playerSections.TryGetValue(oldSection, out var oldSet))
            {
                oldSet.TryRemove(playerId);
            }
        }

        // 새 섹션에 추가
        var newSet = _playerSections.GetOrAdd(newSection, _ => new ConcurrentHashSet<int>());
        newSet.Add(playerId);
        _playerPositions[playerId] = newSection;

        return true;
    }

    public async Task<bool> MoveEnemyToSectionAsync(int enemyId, SectionId newSection)
    {
        if (_enemyPositions.TryGetValue(enemyId, out var oldSection))
        {
            if (oldSection == newSection) return false;

            if (_enemySections.TryGetValue(oldSection, out var oldSet))
            {
                oldSet.TryRemove(enemyId);
            }
        }

        var newSet = _enemySections.GetOrAdd(newSection, _ => new ConcurrentHashSet<int>());
        newSet.Add(enemyId);
        _enemyPositions[enemyId] = newSection;

        return true;
    }

    public IEnumerable<int> GetPlayersInSection(SectionId section)
    {
        return _playerSections.TryGetValue(section, out var players) ? players.ToArray() : Array.Empty<int>();
    }

    public IEnumerable<int> GetEnemiesInSection(SectionId section)
    {
        return _enemySections.TryGetValue(section, out var enemies) ? enemies.ToArray() : Array.Empty<int>();
    }

    public IEnumerable<int> GetNearbyPlayers(SectionId center, int radius = 1)
    {
        var nearbyPlayers = new HashSet<int>();
        foreach (var section in center.GetNeighbors(radius))
        {
            foreach (var playerId in GetPlayersInSection(section))
            {
                nearbyPlayers.Add(playerId);
            }
        }
        return nearbyPlayers;
    }

    public IEnumerable<int> GetNearbyEnemies(SectionId center, int radius = 1)
    {
        var nearbyEnemies = new HashSet<int>();
        foreach (var section in center.GetNeighbors(radius))
        {
            foreach (var enemyId in GetEnemiesInSection(section))
            {
                nearbyEnemies.Add(enemyId);
            }
        }
        return nearbyEnemies;
    }
}

// Thread-Safe HashSet
public class ConcurrentHashSet<T> : IDisposable
{
    private readonly HashSet<T> _hashSet = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public bool Add(T item)
    {
        _lock.EnterWriteLock();
        try { return _hashSet.Add(item); }
        finally { _lock.ExitWriteLock(); }
    }

    public bool TryRemove(T item)
    {
        _lock.EnterWriteLock();
        try { return _hashSet.Remove(item); }
        finally { _lock.ExitWriteLock(); }
    }

    public T[] ToArray()
    {
        _lock.EnterReadLock();
        try { return _hashSet.ToArray(); }
        finally { _lock.ExitReadLock(); }
    }

    public void Dispose() => _lock?.Dispose();
}

// ===== 4. 스폰 시스템 =====

public class SpawnManager
{
    private readonly IGameRepository _repository;
    private readonly SpatialManager _spatialManager;
    private readonly Subject<PlayerSpawnEvent> _playerSpawnSubject = new();
    private readonly Subject<EnemySpawnEvent> _enemySpawnSubject = new();
    private readonly Timer _enemySpawnTimer;
    private readonly Random _random = new();

    public IObservable<PlayerSpawnEvent> PlayerSpawns => _playerSpawnSubject.AsObservable();
    public IObservable<EnemySpawnEvent> EnemySpawns => _enemySpawnSubject.AsObservable();

    public SpawnManager(IGameRepository repository, SpatialManager spatialManager)
    {
        _repository = repository;
        _spatialManager = spatialManager;

        // 주기적인 적 스폰 (30초마다)
        _enemySpawnTimer = new Timer(async _ => await SpawnEnemiesAsync(), null,
            TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    public async Task<Player> SpawnPlayerAsync(string playerName)
    {
        try
        {
            // 플레이어 스폰 포인트 가져오기
            var spawnPoints = await _repository.GetSpawnPointsAsync("player");
            if (!spawnPoints.Any())
            {
                // 기본 스폰 포인트
                spawnPoints = new[] { new Vector3(0, 0, 0) };
            }

            var spawnPoint = spawnPoints[_random.Next(spawnPoints.Length)];
            var player = await _repository.CreatePlayerAsync(playerName, spawnPoint);

            // 공간 분할 시스템에 등록
            await _spatialManager.MovePlayerToSectionAsync(player.PlayerId, player.Section);

            // 스폰 이벤트 발행
            _playerSpawnSubject.OnNext(new PlayerSpawnEvent(DateTime.UtcNow, player.Section, player));

            return player;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Player spawn error: {ex.Message}");
            throw;
        }
    }

    private async Task SpawnEnemiesAsync()
    {
        try
        {
            var enemyTypes = new[] { "goblin", "orc", "dragon" };
            var spawnTasks = new List<Task>();

            foreach (var enemyType in enemyTypes)
            {
                spawnTasks.Add(SpawnEnemyTypeAsync(enemyType));
            }

            await Task.WhenAll(spawnTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Enemy spawn error: {ex.Message}");
        }
    }

    private async Task SpawnEnemyTypeAsync(string enemyType)
    {
        var spawnPoints = await _repository.GetSpawnPointsAsync(enemyType);
        if (!spawnPoints.Any()) return;

        var maxSpawns = enemyType switch
        {
            "goblin" => 10,
            "orc" => 5,
            "dragon" => 1,
            _ => 3
        };

        for (int i = 0; i < maxSpawns; i++)
        {
            var spawnPoint = spawnPoints[_random.Next(spawnPoints.Length)];
            var section = _spatialManager.GetSection(spawnPoint);

            // 해당 섹션에 이미 적이 많으면 스킵
            var existingEnemies = _spatialManager.GetEnemiesInSection(section);
            if (existingEnemies.Count() >= 5) continue;

            var enemy = await _repository.CreateEnemyAsync(enemyType, spawnPoint, section);
            await _spatialManager.MoveEnemyToSectionAsync(enemy.EnemyId, enemy.Section);

            _enemySpawnSubject.OnNext(new EnemySpawnEvent(DateTime.UtcNow, enemy.Section, enemy));
        }
    }

    public void Dispose()
    {
        _enemySpawnTimer?.Dispose();
        _playerSpawnSubject?.Dispose();
        _enemySpawnSubject?.Dispose();
    }
}

// ===== 5. 움직임 처리 시스템 =====

public class MovementProcessor
{
    private readonly ActionBlock<PlayerMoveRequest> _playerMoveQueue;
    private readonly ActionBlock<(int enemyId, Vector3 targetPosition)> _enemyMoveQueue;
    private readonly ConcurrentDictionary<int, Player> _players = new();
    private readonly ConcurrentDictionary<int, Enemy> _enemies = new();
    private readonly SpatialManager _spatialManager;
    private readonly IGameRepository _repository;
    private readonly Subject<PlayerMoveEvent> _playerMoveSubject = new();
    private readonly Subject<EnemyMoveEvent> _enemyMoveSubject = new();

    public IObservable<PlayerMoveEvent> PlayerMoves => _playerMoveSubject.AsObservable();
    public IObservable<EnemyMoveEvent> EnemyMoves => _enemyMoveSubject.AsObservable();

    public MovementProcessor(SpatialManager spatialManager, IGameRepository repository)
    {
        _spatialManager = spatialManager;
        _repository = repository;

        // 플레이어 움직임 처리 파이프라인
        _playerMoveQueue = new ActionBlock<PlayerMoveRequest>(
            async moveRequest => await ProcessPlayerMoveAsync(moveRequest),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = 10000
            });

        // 적 움직임 처리 파이프라인
        _enemyMoveQueue = new ActionBlock<(int enemyId, Vector3 targetPosition)>(
            async move => await ProcessEnemyMoveAsync(move.enemyId, move.targetPosition),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount / 2,
                BoundedCapacity = 5000
            });
    }

    public async Task<bool> RequestPlayerMoveAsync(PlayerMoveRequest moveRequest)
    {
        return await _playerMoveQueue.SendAsync(moveRequest);
    }

    public async Task<bool> RequestEnemyMoveAsync(int enemyId, Vector3 targetPosition)
    {
        return await _enemyMoveQueue.SendAsync((enemyId, targetPosition));
    }

    private async Task ProcessPlayerMoveAsync(PlayerMoveRequest request)
    {
        if (!_players.TryGetValue(request.ClientId, out var player))
            return;

        try
        {
            // 이동 검증
            var distance = player.Position.DistanceTo(request.Position);
            var deltaTime = (request.Timestamp - player.LastUpdate).TotalSeconds;
            var maxDistance = 20.0f * deltaTime; // 최대 속도 제한

            if (distance > maxDistance)
            {
                Console.WriteLine($"Player {request.ClientId} movement rejected: too fast");
                return;
            }

            var oldPosition = player.Position;
            var newSection = _spatialManager.GetSection(request.Position);

            // 플레이어 상태 업데이트
            var updatedPlayer = player with
            {
                Position = request.Position,
                Velocity = request.Velocity,
                Section = newSection,
                LastUpdate = request.Timestamp
            };

            _players[request.ClientId] = updatedPlayer;

            // 섹션 변경 처리
            if (player.Section != newSection)
            {
                await _spatialManager.MovePlayerToSectionAsync(request.ClientId, newSection);
            }

            // 데이터베이스 업데이트 (비동기)
            _ = Task.Run(async () => await _repository.UpdatePlayerAsync(updatedPlayer));

            // 이동 이벤트 발행
            _playerMoveSubject.OnNext(new PlayerMoveEvent(
                request.Timestamp, newSection, request.ClientId, oldPosition, request.Position));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Player move processing error: {ex.Message}");
        }
    }

    private async Task ProcessEnemyMoveAsync(int enemyId, Vector3 targetPosition)
    {
        if (!_enemies.TryGetValue(enemyId, out var enemy))
            return;

        try
        {
            var oldPosition = enemy.Position;
            var direction = targetPosition.Add(enemy.Position.Multiply(-1)).Normalize();
            var speed = 5.0f; // 적의 이동 속도
            var deltaTime = 0.1f; // 100ms

            var newPosition = enemy.Position.Add(direction.Multiply(speed * deltaTime));
            var newSection = _spatialManager.GetSection(newPosition);

            var updatedEnemy = enemy with
            {
                Position = newPosition,
                Velocity = direction.Multiply(speed),
                Section = newSection,
                LastUpdate = DateTime.UtcNow
            };

            _enemies[enemyId] = updatedEnemy;

            // 섹션 변경 처리
            if (enemy.Section != newSection)
            {
                await _spatialManager.MoveEnemyToSectionAsync(enemyId, newSection);
            }

            // 데이터베이스 업데이트 (비동기)
            _ = Task.Run(async () => await _repository.UpdateEnemyAsync(updatedEnemy));

            // 이동 이벤트 발행
            _enemyMoveSubject.OnNext(new EnemyMoveEvent(
                DateTime.UtcNow, newSection, enemyId, oldPosition, newPosition));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Enemy move processing error: {ex.Message}");
        }
    }

    public void AddPlayer(Player player)
    {
        _players[player.PlayerId] = player;
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies[enemy.EnemyId] = enemy;
    }

    public Player? GetPlayer(int playerId)
    {
        return _players.TryGetValue(playerId, out var player) ? player : null;
    }

    public Enemy? GetEnemy(int enemyId)
    {
        return _enemies.TryGetValue(enemyId, out var enemy) ? enemy : null;
    }

    public void Dispose()
    {
        _playerMoveQueue?.Complete();
        _enemyMoveQueue?.Complete();
        _playerMoveSubject?.Dispose();
        _enemyMoveSubject?.Dispose();
    }
}

// ===== 6. 충돌 감지 및 처리 시스템 =====

public class CollisionManager
{
    private readonly SpatialManager _spatialManager;
    private readonly MovementProcessor _movementProcessor;
    private readonly IGameRepository _repository;
    private readonly Timer _collisionTimer;
    private readonly Subject<CollisionEvent> _collisionSubject = new();
    private readonly Subject<CombatEvent> _combatSubject = new();

    public IObservable<CollisionEvent> Collisions => _collisionSubject.AsObservable();
    public IObservable<CombatEvent> Combat => _combatSubject.AsObservable();

    public CollisionManager(SpatialManager spatialManager, MovementProcessor movementProcessor, IGameRepository repository)
    {
        _spatialManager = spatialManager;
        _movementProcessor = movementProcessor;
        _repository = repository;

        // 50ms마다 충돌 감지 (20 FPS)
        _collisionTimer = new Timer(async _ => await DetectCollisionsAsync(), null,
            TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50));
    }

    private async Task DetectCollisionsAsync()
    {
        try
        {
            var collisionTasks = new List<Task>();

            // 모든 활성 섹션에서 충돌 감지
            for (int x = -10; x <= 10; x++)
            {
                for (int y = -10; y <= 10; y++)
                {
                    var section = new SectionId(x, y);
                    collisionTasks.Add(DetectSectionCollisionsAsync(section));
                }
            }

            await Task.WhenAll(collisionTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Collision detection error: {ex.Message}");
        }
    }

    private async Task DetectSectionCollisionsAsync(SectionId section)
    {
        var playerIds = _spatialManager.GetPlayersInSection(section).ToArray();
        var enemyIds = _spatialManager.GetEnemiesInSection(section).ToArray();

        if (!playerIds.Any() && !enemyIds.Any()) return;

        var players = playerIds.Select(id => _movementProcessor.GetPlayer(id))
                              .Where(p => p != null && p.IsAlive)
                              .ToArray();

        var enemies = enemyIds.Select(id => _movementProcessor.GetEnemy(id))
                              .Where(e => e != null && e.IsAlive)
                              .ToArray();

        // 플레이어 vs 적 충돌
        await Task.Run(() =>
        {
            Parallel.ForEach(players, player =>
            {
                foreach (var enemy in enemies)
                {
                    var distance = player!.Position.DistanceTo(enemy!.Position);
                    if (distance <= player.CollisionRadius + enemy.CollisionRadius)
                    {
                        var collisionPoint = new Vector3(
                            (player.Position.X + enemy.Position.X) / 2,
                            (player.Position.Y + enemy.Position.Y) / 2,
                            (player.Position.Z + enemy.Position.Z) / 2
                        );

                        var collision = new CollisionEvent(
                            DateTime.UtcNow, section, player.PlayerId, enemy.EnemyId,
                            EntityType.Player, EntityType.Enemy, collisionPoint);

                        _collisionSubject.OnNext(collision);

                        // 전투 이벤트 발생
                        if (enemy.CanAttack && enemy.TargetPlayerId == player.PlayerId)
                        {
                            var combat = new CombatEvent(
                                DateTime.UtcNow, section, enemy.EnemyId, player.PlayerId,
                                EntityType.Enemy, EntityType.Player, enemy.Damage);

                            _combatSubject.OnNext(combat);
                        }
                    }
                }
            });
        });

        // 플레이어 vs 플레이어 충돌 (PvP)
        await Task.Run(() =>
        {
            Parallel.For(0, players.Length, i =>
            {
                for (int j = i + 1; j < players.Length; j++)
                {
                    var player1 = players[i]!;
                    var player2 = players[j]!;

                    var distance = player1.Position.DistanceTo(player2.Position);
                    if (distance <= player1.CollisionRadius + player2.CollisionRadius)
                    {
                        var collisionPoint = new Vector3(
                            (player1.Position.X + player2.Position.X) / 2,
                            (player1.Position.Y + player2.Position.Y) / 2,
                            (player1.Position.Z + player2.Position.Z) / 2
                        );

                        var collision = new CollisionEvent(
                            DateTime.UtcNow, section, player1.PlayerId, player2.PlayerId,
                            EntityType.Player, EntityType.Player, collisionPoint);

                        _collisionSubject.OnNext(collision);
                    }
                }
            });
        });
    }

    public void Dispose()
    {
        _collisionTimer?.Dispose();
        _collisionSubject?.Dispose();
        _combatSubject?.Dispose();
    }
}

// ===== 7. 전투 시스템 =====

public class CombatSystem
{
    private readonly MovementProcessor _movementProcessor;
    private readonly IGameRepository _repository;
    private readonly Subject<PlayerDeathEvent> _playerDeathSubject = new();
    private readonly Subject<EnemyDeathEvent> _enemyDeathSubject = new();

    public IObservable<PlayerDeathEvent> PlayerDeaths => _playerDeathSubject.AsObservable();
    public IObservable<EnemyDeathEvent> EnemyDeaths => _enemyDeathSubject.AsObservable();

    public CombatSystem(MovementProcessor movementProcessor, IGameRepository repository)
    {
        _movementProcessor = movementProcessor;
        _repository = repository;
    }

    public async Task ProcessCombatAsync(CombatEvent combat)
    {
        try
        {
            await _repository.LogCombatEventAsync(combat);

            if (combat.TargetType == EntityType.Player)
            {
                await ProcessPlayerDamageAsync(combat);
            }
            else if (combat.TargetType == EntityType.Enemy)
            {
                await ProcessEnemyDamageAsync(combat);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Combat processing error: {ex.Message}");
        }
    }

    private async Task ProcessPlayerDamageAsync(CombatEvent combat)
    {
        var player = _movementProcessor.GetPlayer(combat.TargetId);
        if (player == null || !player.IsAlive) return;

        var newHealth = Math.Max(0, player.Health - combat.Damage);
        var updatedPlayer = player with { Health = newHealth };

        _movementProcessor.AddPlayer(updatedPlayer);
        await _repository.UpdatePlayerAsync(updatedPlayer);

        if (newHealth <= 0)
        {
            var deathEvent = new PlayerDeathEvent(
                DateTime.UtcNow, player.Section, player.PlayerId,
                combat.AttackerId, combat.AttackerType);

            _playerDeathSubject.OnNext(deathEvent);
        }
    }

    private async Task ProcessEnemyDamageAsync(CombatEvent combat)
    {
        var enemy = _movementProcessor.GetEnemy(combat.TargetId);
        if (enemy == null || !enemy.IsAlive) return;

        var newHealth = Math.Max(0, enemy.Health - combat.Damage);
        var updatedEnemy = enemy with { Health = newHealth };

        _movementProcessor.AddEnemy(updatedEnemy);
        await _repository.UpdateEnemyAsync(updatedEnemy);

        if (newHealth <= 0)
        {
            var deathEvent = new EnemyDeathEvent(
                DateTime.UtcNow, enemy.Section, enemy.EnemyId, combat.AttackerId);

            _enemyDeathSubject.OnNext(deathEvent);
        }
    }

    public void Dispose()
    {
        _playerDeathSubject?.Dispose();
        _enemyDeathSubject?.Dispose();
    }
}

// ===== 8. 네트워크 통신 레이어 =====

public class NetworkManager
{
    private readonly TcpListener _tcpListener;
    private readonly ConcurrentDictionary<int, NetworkClient> _clients = new();
    private readonly Channel<NetworkMessage> _incomingMessages;
    private readonly Channel<NetworkMessage> _outgoingMessages;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private int _nextClientId = 1;

    public NetworkManager(IPEndPoint endpoint)
    {
        _tcpListener = new TcpListener(endpoint);

        var channelOptions = new BoundedChannelOptions(50000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };

        _incomingMessages = Channel.CreateBounded<NetworkMessage>(channelOptions);
        _outgoingMessages = Channel.CreateBounded<NetworkMessage>(channelOptions);
    }

    public async Task StartAsync()
    {
        _tcpListener.Start();
        Console.WriteLine($"Network server started on {_tcpListener.LocalEndpoint}");

        // 클라이언트 연결 수락
        _ = Task.Run(AcceptClientsAsync);

        // 나가는 메시지 처리
        _ = Task.Run(ProcessOutgoingMessagesAsync);
    }

    private async Task AcceptClientsAsync()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                var clientId = Interlocked.Increment(ref _nextClientId);

                var networkClient = new NetworkClient(clientId, tcpClient);
                _clients.TryAdd(clientId, networkClient);

                // 클라이언트별 메시지 처리
                _ = Task.Run(async () => await ProcessClientAsync(networkClient));

                Console.WriteLine($"Client {clientId} connected from {tcpClient.Client.RemoteEndPoint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Accept client error: {ex.Message}");
            }
        }
    }

    private async Task ProcessClientAsync(NetworkClient client)
    {
        var buffer = new byte[4096];
        var stream = client.TcpClient.GetStream();

        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested && client.TcpClient.Connected)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token);
                if (bytesRead == 0) break;

                var messageJson = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var message = DeserializeMessage(messageJson, client.ClientId);

                if (message != null)
                {
                    await _incomingMessages.Writer.WriteAsync(message, _cancellationTokenSource.Token);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client {client.ClientId} processing error: {ex.Message}");
        }
        finally
        {
            _clients.TryRemove(client.ClientId, out _);
            client.TcpClient.Close();
            Console.WriteLine($"Client {client.ClientId} disconnected");
        }
    }

    private async Task ProcessOutgoingMessagesAsync()
    {
        await foreach (var message in _outgoingMessages.Reader.ReadAllAsync(_cancellationTokenSource.Token))
        {
            try
            {
                if (_clients.TryGetValue(message.ClientId, out var client) && client.TcpClient.Connected)
                {
                    var messageJson = JsonSerializer.Serialize(message);
                    var messageBytes = System.Text.Encoding.UTF8.GetBytes(messageJson);

                    var stream = client.TcpClient.GetStream();
                    await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send message error: {ex.Message}");
            }
        }
    }

    public async IAsyncEnumerable<NetworkMessage> GetIncomingMessagesAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var message in _incomingMessages.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
    }

    public async Task SendMessageAsync(NetworkMessage message)
    {
        await _outgoingMessages.Writer.WriteAsync(message);
    }

    public async Task BroadcastMessageAsync(NetworkMessage messageTemplate, int[] clientIds)
    {
        var tasks = clientIds.Select(async clientId =>
        {
            var message = messageTemplate with { ClientId = clientId };
            await SendMessageAsync(message);
        });

        await Task.WhenAll(tasks);
    }

    private NetworkMessage? DeserializeMessage(string json, int clientId)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (!root.TryGetProperty("Type", out var typeElement))
                return null;

            var messageType = (MessageType)typeElement.GetInt32();
            var timestamp = DateTime.UtcNow;

            return messageType switch
            {
                MessageType.PlayerJoin => new PlayerJoinRequest(
                    clientId, timestamp, root.GetProperty("PlayerName").GetString() ?? ""),
                MessageType.PlayerMove => new PlayerMoveRequest(
                    clientId, timestamp,
                    JsonSerializer.Deserialize<Vector3>(root.GetProperty("Position").GetRawText()),
                    JsonSerializer.Deserialize<Vector3>(root.GetProperty("Velocity").GetRawText())),
                MessageType.PlayerAttack => new PlayerAttackRequest(
                    clientId, timestamp,
                    root.GetProperty("TargetId").GetInt32(),
                    (EntityType)root.GetProperty("TargetType").GetInt32()),
                _ => null
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Message deserialization error: {ex.Message}");
            return null;
        }
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _tcpListener?.Stop();

        foreach (var client in _clients.Values)
        {
            client.TcpClient.Close();
        }

        _incomingMessages.Writer.Complete();
        _outgoingMessages.Writer.Complete();
    }

    public void Dispose()
    {
        Stop();
        _cancellationTokenSource?.Dispose();
    }
}

public class NetworkClient
{
    public int ClientId { get; }
    public TcpClient TcpClient { get; }
    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;

    public NetworkClient(int clientId, TcpClient tcpClient)
    {
        ClientId = clientId;
        TcpClient = tcpClient;
    }
}

// ===== 9. 실시간 게임 상태 스트리밍 =====

public class GameStateStreamer
{
    private readonly MovementProcessor _movementProcessor;
    private readonly SpatialManager _spatialManager;
    private readonly NetworkManager _networkManager;
    private readonly Timer _stateUpdateTimer;

    public GameStateStreamer(MovementProcessor movementProcessor, SpatialManager spatialManager, NetworkManager networkManager)
    {
        _movementProcessor = movementProcessor;
        _spatialManager = spatialManager;
        _networkManager = networkManager;

        // 30 FPS로 게임 상태 업데이트
        _stateUpdateTimer = new Timer(async _ => await BroadcastGameStateAsync(), null,
            TimeSpan.FromMilliseconds(33), TimeSpan.FromMilliseconds(33));
    }

    private async Task BroadcastGameStateAsync()
    {
        try
        {
            var updateTasks = new List<Task>();

            // 각 섹션별로 상태 업데이트
            for (int x = -10; x <= 10; x++)
            {
                for (int y = -10; y <= 10; y++)
                {
                    var section = new SectionId(x, y);
                    updateTasks.Add(BroadcastSectionStateAsync(section));
                }
            }

            await Task.WhenAll(updateTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Game state broadcast error: {ex.Message}");
        }
    }

    private async Task BroadcastSectionStateAsync(SectionId section)
    {
        var playerIds = _spatialManager.GetPlayersInSection(section).ToArray();
        var enemyIds = _spatialManager.GetNearbyEnemies(section, 2).ToArray();

        if (!playerIds.Any()) return;

        var players = playerIds.Select(id => _movementProcessor.GetPlayer(id))
                              .Where(p => p != null && p.IsAlive)
                              .ToArray();

        var enemies = enemyIds.Select(id => _movementProcessor.GetEnemy(id))
                              .Where(e => e != null && e.IsAlive)
                              .ToArray();

        // 각 플레이어에게 시야 내 상태 전송
        foreach (var player in players)
        {
            if (player == null) continue;

            var visiblePlayers = players.Where(p => p != null && p.PlayerId != player.PlayerId &&
                                               IsInViewDistance(player.Position, p.Position, 50.0f))
                                        .ToArray();

            var visibleEnemies = enemies.Where(e => e != null &&
                                               IsInViewDistance(player.Position, e.Position, 50.0f))
                                        .ToArray();

            var stateUpdate = new GameStateUpdate(
                player.PlayerId, DateTime.UtcNow, visiblePlayers!, visibleEnemies!);

            await _networkManager.SendMessageAsync(stateUpdate);
        }
    }

    private static bool IsInViewDistance(Vector3 pos1, Vector3 pos2, float maxDistance)
    {
        return pos1.DistanceTo(pos2) <= maxDistance;
    }

    public void Dispose()
    {
        _stateUpdateTimer?.Dispose();
    }
}

// ===== 10. AI 시스템 (적 행동) =====

public class AISystem
{
    private readonly MovementProcessor _movementProcessor;
    private readonly SpatialManager _spatialManager;
    private readonly Timer _aiUpdateTimer;
    private readonly Subject<CombatEvent> _aiCombatSubject = new();

    public IObservable<CombatEvent> AICombat => _aiCombatSubject.AsObservable();

    public AISystem(MovementProcessor movementProcessor, SpatialManager spatialManager)
    {
        _movementProcessor = movementProcessor;
        _spatialManager = spatialManager;

        // 100ms마다 AI 업데이트 (10 FPS)
        _aiUpdateTimer = new Timer(async _ => await UpdateAIAsync(), null,
            TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
    }

    private async Task UpdateAIAsync()
    {
        try
        {
            var aiTasks = new List<Task>();

            for (int x = -10; x <= 10; x++)
            {
                for (int y = -10; y <= 10; y++)
                {
                    var section = new SectionId(x, y);
                    aiTasks.Add(UpdateSectionAIAsync(section));
                }
            }

            await Task.WhenAll(aiTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI update error: {ex.Message}");
        }
    }

    private async Task UpdateSectionAIAsync(SectionId section)
    {
        var enemyIds = _spatialManager.GetEnemiesInSection(section).ToArray();
        var playerIds = _spatialManager.GetNearbyPlayers(section, 2).ToArray();

        if (!enemyIds.Any()) return;

        var enemies = enemyIds.Select(id => _movementProcessor.GetEnemy(id))
                              .Where(e => e != null && e.IsAlive)
                              .ToArray();

        var players = playerIds.Select(id => _movementProcessor.GetPlayer(id))
                               .Where(p => p != null && p.IsAlive)
                               .ToArray();

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            await UpdateEnemyAIAsync(enemy, players!);
        }
    }

    private async Task UpdateEnemyAIAsync(Enemy enemy, Player[] nearbyPlayers)
    {
        var closestPlayer = nearbyPlayers
            .Where(p => enemy.Position.DistanceTo(p.Position) <= enemy.DetectionRange)
            .OrderBy(p => enemy.Position.DistanceTo(p.Position))
            .FirstOrDefault();

        var newState = enemy.State;
        var newTargetPlayerId = enemy.TargetPlayerId;

        if (closestPlayer != null)
        {
            var distanceToPlayer = enemy.Position.DistanceTo(closestPlayer.Position);

            if (distanceToPlayer <= enemy.AttackRange)
            {
                // 공격 범위 내 - 공격
                newState = EnemyState.Attacking;
                newTargetPlayerId = closestPlayer.PlayerId;

                // 공격 이벤트 발생
                var combat = new CombatEvent(
                    DateTime.UtcNow, enemy.Section, enemy.EnemyId, closestPlayer.PlayerId,
                    EntityType.Enemy, EntityType.Player, enemy.Damage);

                _aiCombatSubject.OnNext(combat);
            }
            else if (distanceToPlayer <= enemy.DetectionRange)
            {
                // 감지 범위 내 - 추적
                newState = EnemyState.Chasing;
                newTargetPlayerId = closestPlayer.PlayerId;

                // 플레이어 방향으로 이동
                await _movementProcessor.RequestEnemyMoveAsync(enemy.EnemyId, closestPlayer.Position);
            }
        }
        else
        {
            // 플레이어가 없음 - 스폰 포인트로 돌아가기
            var distanceToSpawn = enemy.Position.DistanceTo(enemy.SpawnPoint);
            if (distanceToSpawn > 20.0f)
            {
                newState = EnemyState.Retreating;
                newTargetPlayerId = null;
                await _movementProcessor.RequestEnemyMoveAsync(enemy.EnemyId, enemy.SpawnPoint);
            }
            else
            {
                newState = EnemyState.Idle;
                newTargetPlayerId = null;
            }
        }

        // 상태 변경이 있으면 업데이트
        if (newState != enemy.State || newTargetPlayerId != enemy.TargetPlayerId)
        {
            var updatedEnemy = enemy with
            {
                State = newState,
                TargetPlayerId = newTargetPlayerId,
                LastUpdate = DateTime.UtcNow
            };

            _movementProcessor.AddEnemy(updatedEnemy);
        }
    }

    public void Dispose()
    {
        _aiUpdateTimer?.Dispose();
        _aiCombatSubject?.Dispose();
    }
}

// ===== 11. 메인 게임 서버 오케스트레이터 =====

public class MMOGameServer : IDisposable
{
    private readonly IGameRepository _repository;
    private readonly SpatialManager _spatialManager;
    private readonly SpawnManager _spawnManager;
    private readonly MovementProcessor _movementProcessor;
    private readonly CollisionManager _collisionManager;
    private readonly CombatSystem _combatSystem;
    private readonly NetworkManager _networkManager;
    private readonly GameStateStreamer _gameStateStreamer;
    private readonly AISystem _aiSystem;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public MMOGameServer(string connectionString, IPEndPoint networkEndpoint)
    {
        // 의존성 주입 및 초기화
        _repository = new GameRepository(connectionString);
        _spatialManager = new SpatialManager();
        _spawnManager = new SpawnManager(_repository, _spatialManager);
        _movementProcessor = new MovementProcessor(_spatialManager, _repository);
        _collisionManager = new CollisionManager(_spatialManager, _movementProcessor, _repository);
        _combatSystem = new CombatSystem(_movementProcessor, _repository);
        _networkManager = new NetworkManager(networkEndpoint);
        _gameStateStreamer = new GameStateStreamer(_movementProcessor, _spatialManager, _networkManager);
        _aiSystem = new AISystem(_movementProcessor, _spatialManager);

        // 이벤트 스트림 연결
        SetupEventStreams();
    }

    private void SetupEventStreams()
    {
        // 플레이어 스폰 이벤트
        _spawnManager.PlayerSpawns.Subscribe(async spawn =>
        {
            _movementProcessor.AddPlayer(spawn.Player);
            Console.WriteLine($"Player {spawn.Player.Name} spawned at {spawn.Player.Position}");
        });

        // 적 스폰 이벤트
        _spawnManager.EnemySpawns.Subscribe(async spawn =>
        {
            _movementProcessor.AddEnemy(spawn.Enemy);
            Console.WriteLine($"Enemy {spawn.Enemy.Type} spawned at {spawn.Enemy.Position}");
        });

        // 충돌 이벤트
        _collisionManager.Collisions.Subscribe(collision =>
        {
            Console.WriteLine($"Collision: {collision.Entity1Type} {collision.Entity1Id} vs {collision.Entity2Type} {collision.Entity2Id}");
        });

        // 전투 이벤트 (플레이어 공격 + AI 공격)
        Observable.Merge(_collisionManager.Combat, _aiSystem.AICombat)
            .Subscribe(async combat => await _combatSystem.ProcessCombatAsync(combat));

        // 전투 업데이트를 네트워크로 브로드캐스트
        _collisionManager.Combat.Subscribe(async combat =>
        {
            var combatUpdate = new CombatUpdate(0, DateTime.UtcNow, combat);
            var nearbyPlayerIds = _spatialManager.GetNearbyPlayers(combat.Section, 2).ToArray();
            await _networkManager.BroadcastMessageAsync(combatUpdate, nearbyPlayerIds);
        });

        // 플레이어 사망 처리
        _combatSystem.PlayerDeaths.Subscribe(death =>
        {
            Console.WriteLine($"Player {death.PlayerId} died");
            // 리스폰 로직 등 추가 가능
        });

        // 적 사망 처리
        _combatSystem.EnemyDeaths.Subscribe(death =>
        {
            Console.WriteLine($"Enemy {death.EnemyId} died");
            // 경험치, 아이템 드롭 등 로직 추가 가능
        });
    }

    public async Task StartAsync()
    {
        Console.WriteLine("🎮 MMO Game Server Starting...");

        // 네트워크 시작
        await _networkManager.StartAsync();

        // 네트워크 메시지 처리
        _ = Task.Run(ProcessNetworkMessagesAsync);

        Console.WriteLine("✅ MMO Game Server started successfully!");
        Console.WriteLine("Press 'q' to quit");

        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.KeyChar == 'q')
            {
                Console.WriteLine("Shutting down...");
                break;
            }
        }
    }

    private async Task ProcessNetworkMessagesAsync()
    {
        await foreach (var message in _networkManager.GetIncomingMessagesAsync(_cancellationTokenSource.Token))
        {
            try
            {
                await ProcessGameMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message processing error: {ex.Message}");
            }
        }
    }

    private async Task ProcessGameMessageAsync(NetworkMessage message)
    {
        switch (message)
        {
            case PlayerJoinRequest joinRequest:
                var player = await _spawnManager.SpawnPlayerAsync(joinRequest.PlayerName);
                break;

            case PlayerMoveRequest moveRequest:
                await _movementProcessor.RequestPlayerMoveAsync(moveRequest);
                break;

            case PlayerAttackRequest attackRequest:
                if (attackRequest.TargetType == EntityType.Enemy)
                {
                    var attacker = _movementProcessor.GetPlayer(attackRequest.ClientId);
                    var target = _movementProcessor.GetEnemy(attackRequest.TargetId);

                    if (attacker != null && target != null)
                    {
                        var distance = attacker.Position.DistanceTo(target.Position);
                        if (distance <= 3.0f) // 공격 범위
                        {
                            var combat = new CombatEvent(
                                DateTime.UtcNow, attacker.Section, attacker.PlayerId, target.EnemyId,
                                EntityType.Player, EntityType.Enemy, 25); // 플레이어 기본 데미지

                            await _combatSystem.ProcessCombatAsync(combat);
                        }
                    }
                }
                break;
        }
    }

    public void Dispose()
    {
        Console.WriteLine("🛑 Disposing MMO Game Server...");

        _cancellationTokenSource?.Cancel();
        _networkManager?.Dispose();
        _gameStateStreamer?.Dispose();
        _aiSystem?.Dispose();
        _collisionManager?.Dispose();
        _combatSystem?.Dispose();
        _movementProcessor?.Dispose();
        _spawnManager?.Dispose();
        _cancellationTokenSource?.Dispose();

        Console.WriteLine("✅ MMO Game Server disposed");
    }
}

// ===== 12. 메인 프로그램 =====

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🎮 MMO Game Server");
        Console.WriteLine("==================");

        var connectionString = "Server=localhost;Database=mmogame;Uid=root;Pwd=password;";
        var networkEndpoint = new IPEndPoint(IPAddress.Any, 8080);

        try
        {
            using var server = new MMOGameServer(connectionString, networkEndpoint);
            await server.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Server error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine("👋 Server shutdown complete.");
    }
}