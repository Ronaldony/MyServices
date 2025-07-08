using System.Reactive.Linq;
using System.Threading.Channels;

namespace TestProject.Datas
{
    /// <summary>
    // Game World Orchestrator
    /// GameWorld.
    /// </summary>
    public class GameWorld
    {
        private readonly MessageRouter _router;
        private readonly LockFreeGridSystem _gridSystem;
        private readonly CollisionDetectionSystem _collisionSystem;
        private readonly Channel<GameEvent> _eventChannel;
        private readonly Timer _frameUpdateTimer;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private long _frameCount = 0;
        private long _eventSequence = 0;

        public GameWorld()
        {
            _router = new MessageRouter();
            _gridSystem = new LockFreeGridSystem();
            _collisionSystem = new CollisionDetectionSystem(_gridSystem, _router);
            _eventChannel = Channel.CreateUnbounded<GameEvent>();
            _cancellationTokenSource = new CancellationTokenSource();

            // 이벤트 스트림 구독
            SubscribeToEvents();

            // 프레임 업데이트 (60 FPS)
            _frameUpdateTimer = new Timer(
                async _ => await UpdateFrameAsync(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMilliseconds(16.67)
            );

            // 이벤트 처리 루프 시작
            _ = ProcessEventsAsync(_cancellationTokenSource.Token);
        }

        private void SubscribeToEvents()
        {
            // 모든 게임 이벤트를 구독
            _router.GetEventStream()
                .Subscribe(async evt =>
                {
                    evt = evt with { SequenceNumber = Interlocked.Increment(ref _eventSequence) };
                    await _eventChannel.Writer.WriteAsync(evt);
                });

            // 이동 이벤트 처리
            _router.GetEventStream()
                .OfType<EntityMovedEvent>()
                .Subscribe(evt =>
                {
                    _gridSystem.UpdateEntityPosition(evt.EntityId, evt.FromPosition, evt.ToPosition);
                    _ = _collisionSystem.CheckCollisionAsync(evt.EntityId, evt.ToPosition, 1.0f);
                });
        }

        private async Task ProcessEventsAsync(CancellationToken cancellationToken)
        {
            await foreach (var gameEvent in _eventChannel.Reader.ReadAllAsync(cancellationToken))
            {
                // Event Sourcing - 모든 이벤트를 순차적으로 처리
                Console.WriteLine($"Event [{gameEvent.SequenceNumber}]: {gameEvent.GetType().Name}");

                // 이벤트 저장, 리플레이 등의 로직을 여기에 추가
                await Task.Yield();
            }
        }

        private async Task UpdateFrameAsync()
        {
            var currentFrame = Interlocked.Increment(ref _frameCount);

            if (currentFrame % 60 == 0) // 1초마다
            {
                Console.WriteLine($"Frame: {currentFrame}, Events: {_eventSequence}");
            }

            // AI 업데이트, 물리 시뮬레이션 등
            await Task.CompletedTask;
        }

        public async Task SpawnPlayerAsync(Vector3 position, string playerName)
        {
            var properties = new Dictionary<string, object>
            {
                ["Name"] = playerName,
                ["Level"] = 1,
                ["Health"] = 100
            };

            await _router.SendMessageAsync(new SpawnMessage(EntityType.Player, position, properties));
        }

        public async Task SpawnEnemyAsync(Vector3 position)
        {
            var properties = new Dictionary<string, object>
            {
                ["Health"] = 50,
                ["AttackPower"] = 10
            };

            await _router.SendMessageAsync(new SpawnMessage(EntityType.Enemy, position, properties));
        }

        public async Task MoveEntityAsync(Guid entityId, Vector3 targetPosition)
        {
            await _router.SendMessageAsync(new MoveMessage(entityId, targetPosition));
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Game World Started!");

            // 초기 플레이어 스폰
            for (int i = 0; i < 10; i++)
            {
                await SpawnPlayerAsync(
                    new Vector3(Random.Shared.NextSingle() * 1000, 0, Random.Shared.NextSingle() * 1000),
                    $"Player_{i}"
                );
            }

            // 초기 적 스폰
            for (int i = 0; i < 50; i++)
            {
                await SpawnEnemyAsync(
                    new Vector3(Random.Shared.NextSingle() * 2000, 0, Random.Shared.NextSingle() * 2000)
                );
            }

            // 시뮬레이션: 랜덤 이동
            _ = Task.Run(async () =>
            {
                var random = new Random();
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(100);

                    // 실제로는 존재하는 엔티티 ID를 사용해야 함
                    var testEntityId = Guid.NewGuid();
                    var targetPos = new Vector3(
                        random.NextSingle() * 1000,
                        0,
                        random.NextSingle() * 1000
                    );
                }
            });
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _frameUpdateTimer?.Dispose();
            _eventChannel.Writer.Complete();
        }
    }

}
