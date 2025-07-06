using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks.Dataflow;

namespace TestProject.Datas
{
    // ===== 5. TPL Dataflow 파이프라인 =====
    /// <summary>
    /// GameProcessingPipeline.
    /// </summary>
    public class GameProcessingPipeline : IDisposable
    {
        // 파이프라인 블록들
        private readonly TransformBlock<MoveRequest, (MoveRequest, bool)> _moveValidator;
        private readonly ActionBlock<(MoveRequest, bool)> _moveProcessor;
        private readonly TransformManyBlock<PlayerState[], CollisionEvent[]> _collisionDetector;
        private readonly ActionBlock<CollisionEvent[]> _collisionHandler;
        private readonly BroadcastBlock<GameEvent> _eventBroadcaster;

        private readonly PerformanceMetrics _metrics;
        private readonly SpatialManager _spatialManager;

        public GameProcessingPipeline(PerformanceMetrics metrics, SpatialManager spatialManager)
        {
            _metrics = metrics;
            _spatialManager = spatialManager;

            // 이동 검증 블록
            _moveValidator = new TransformBlock<MoveRequest, (MoveRequest, bool)>(
                async request => (request, await ValidateMoveAsync(request)),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount * 2,
                    BoundedCapacity = 10000
                });

            // 이동 처리 블록
            _moveProcessor = new ActionBlock<(MoveRequest, bool)>(
                async move => await ProcessValidatedMoveAsync(move.Item1, move.Item2),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    BoundedCapacity = 5000
                });

            // 충돌 감지 블록
            _collisionDetector = new TransformManyBlock<PlayerState[], CollisionEvent[]>(
                async players => await DetectCollisionsAsync(players),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount / 2,
                    BoundedCapacity = 1000
                });

            // 충돌 처리 블록
            _collisionHandler = new ActionBlock<CollisionEvent[]>(
                async collisions => await HandleCollisionsAsync(collisions),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 4,
                    BoundedCapacity = 500
                });

            // 이벤트 브로드캐스터
            _eventBroadcaster = new BroadcastBlock<GameEvent>(evt => evt);

            // 파이프라인 연결
            _moveValidator.LinkTo(_moveProcessor, new DataflowLinkOptions { PropagateCompletion = true });
            _collisionDetector.LinkTo(_collisionHandler, new DataflowLinkOptions { PropagateCompletion = true });
        }

        public async Task<bool> ProcessMoveAsync(MoveRequest request)
        {
            _metrics.IncrementCounter("MovesRequested");
            return await _moveValidator.SendAsync(request);
        }

        public async Task<bool> ProcessCollisionDetectionAsync(PlayerState[] players)
        {
            
            return await _collisionDetector.SendAsync(players);
        }

        private async Task<bool> ValidateMoveAsync(MoveRequest request)
        {
            var sw = Stopwatch.StartNew();

            // 비동기 검증 로직
            await Task.Delay(1); // DB 조회 시뮬레이션

            // 기본 검증 (속도, 지형 등)
            bool isValid = request.TargetPosition.X >= -1000 && request.TargetPosition.X <= 1000 &&
                          request.TargetPosition.Z >= -1000 && request.TargetPosition.Z <= 1000;

            _metrics.RecordTime("MoveValidation", sw.ElapsedMilliseconds);
            return isValid;
        }

        private async Task ProcessValidatedMoveAsync(MoveRequest request, bool isValid)
        {
            if (!isValid)
            {
                _metrics.IncrementCounter("MovesRejected");
                return;
            }

            _metrics.IncrementCounter("MovesProcessed");

            // Section 업데이트
            var newSection = _spatialManager.GetSection(request.TargetPosition);
            await _spatialManager.MovePlayerToSectionAsync(request.PlayerId, newSection);

            // 이벤트 발행
            var moveEvent = new PlayerMoveEvent(DateTime.UtcNow, request.PlayerId, Vector3.Zero, request.TargetPosition);
            await _eventBroadcaster.SendAsync(moveEvent);
        }

        private async Task<CollisionEvent[][]> DetectCollisionsAsync(PlayerState[] players)
        {
            var sw = Stopwatch.StartNew();
            var collisions = new ConcurrentBag<CollisionEvent>();

            // 병렬 충돌 감지
            await Task.Run(() =>
            {
                Parallel.For(0, players.Length, new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, i =>
                {
                    for (int j = i + 1; j < players.Length; j++)
                    {
                        var player1 = players[i];
                        var player2 = players[j];

                        var distance = player1.Position.DistanceTo(player2.Position);
                        if (distance <= PlayerState.CollisionRadius * 2)
                        {
                            var collisionPoint = new Vector3(
                                (player1.Position.X + player2.Position.X) / 2,
                                (player1.Position.Y + player2.Position.Y) / 2,
                                (player1.Position.Z + player2.Position.Z) / 2
                            );

                            collisions.Add(new CollisionEvent(DateTime.UtcNow, player1.PlayerId, player2.PlayerId, collisionPoint));
                        }
                    }
                });
            });

            _metrics.RecordTime("CollisionDetection", sw.ElapsedMilliseconds);
            _metrics.IncrementCounter("CollisionsDetected");

            return new[] { collisions.ToArray() };
        }

        private async Task HandleCollisionsAsync(CollisionEvent[] collisions)
        {
            foreach (var collision in collisions)
            {
                await _eventBroadcaster.SendAsync(collision);
            }

            _metrics.IncrementCounter("CollisionsHandled");
        }

        public void Dispose()
        {
            _moveValidator?.Complete();
            _moveProcessor?.Complete();
            _collisionDetector?.Complete();
            _collisionHandler?.Complete();
            _eventBroadcaster?.Complete();
        }
    }
}
