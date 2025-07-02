namespace TestProject.Datas
{
    // ===== 8. 메인 게임 서버 =====
    /// <summary>
    /// AdvancedGameServer.
    /// </summary>
    public class AdvancedGameServer : IDisposable
    {
        private readonly PerformanceMetrics _metrics = new();
        private readonly SpatialManager _spatialManager = new();
        private readonly EventStreamManager _eventManager = new();
        private readonly GameProcessingPipeline _pipeline;
        private readonly RealTimeUpdateStreamer _updateStreamer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        // 성능 모니터링
        private readonly Timer _metricsTimer;
        private readonly Timer _collisionTimer;

        public AdvancedGameServer()
        {
            _pipeline = new GameProcessingPipeline(_metrics, _spatialManager);
            _updateStreamer = new RealTimeUpdateStreamer(_spatialManager, _eventManager);

            // 이벤트 스트림 구독
            SetupEventSubscriptions();

            // 주기적인 성능 메트릭 출력
            _metricsTimer = new Timer(_ => _metrics.PrintMetrics(), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

            // 주기적인 충돌 감지
            _collisionTimer = new Timer(async _ => await RunCollisionDetectionAsync(), null, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50));
        }

        private void SetupEventSubscriptions()
        {
            // 의심스러운 플레이어 모니터링
            _eventManager.GetSpeedingPlayers()
                .Subscribe(moveEvent =>
                {
                    Console.WriteLine($"⚠️  Speeding player detected: {moveEvent.PlayerId}");
                    _metrics.IncrementCounter("SpeedingPlayersDetected");
                });

            // 빈번한 충돌 모니터링
            _eventManager.GetFrequentCollisions()
                .Subscribe(collision =>
                {
                    Console.WriteLine($"⚠️  Frequent collision: Player {collision.PlayerId} vs {collision.OtherPlayerId}");
                    _metrics.IncrementCounter("FrequentCollisionsDetected");
                });

            // Section 변경 이벤트 처리
            _eventManager.GetEventStream<SectionChangeEvent>()
                .Subscribe(async evt =>
                {
                    Console.WriteLine($"📍 Player {evt.PlayerId} moved to section {evt.To}");
                    await _spatialManager.MovePlayerToSectionAsync(evt.PlayerId, evt.To);
                });
        }

        public async Task StartAsync()
        {
            Console.WriteLine("🚀 Advanced MMO Game Server Starting...");

            // 예제 플레이어들 추가
            await AddTestPlayersAsync();

            // 이동 시뮬레이션 시작
            _ = Task.Run(SimulatePlayerMovementAsync);

            // 실시간 업데이트 스트림 시작
            _ = Task.Run(StreamUpdatesAsync);

            Console.WriteLine("✅ Server started successfully!");
            Console.WriteLine("📊 Monitoring metrics every 10 seconds...");
            Console.WriteLine("🎮 Simulating player movements...");
            Console.WriteLine("Press 'q' to quit, 's' to show stats");

            // 메인 루프
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'q')
                {
                    break;
                }
                else if (key.KeyChar == 's')
                {
                    _metrics.PrintMetrics();
                    PrintServerStats();
                }
            }

            Console.WriteLine("🛑 Server shutting down...");
        }

        private async Task AddTestPlayersAsync()
        {
            var random = new Random();

            // 100명의 테스트 플레이어 추가
            var tasks = Enumerable.Range(1, 100).Select(async playerId =>
            {
                var spawnPoint = new Vector3(
                    random.NextSingle() * 200 - 100,
                    0,
                    random.NextSingle() * 200 - 100
                );

                _updateStreamer.AddPlayer(playerId, spawnPoint);

                var joinEvent = new PlayerJoinEvent(DateTime.UtcNow, playerId, spawnPoint);
                _eventManager.PublishEvent(joinEvent);

                await Task.Delay(10); // 점진적 추가
            });

            await Task.WhenAll(tasks);
            Console.WriteLine($"✅ Added 100 test players");
        }

        private async Task SimulatePlayerMovementAsync()
        {
            var random = new Random();
            int sequenceId = 0;

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    // 랜덤한 플레이어들의 이동 시뮬레이션
                    var moveTasks = Enumerable.Range(1, Math.Min(50, random.Next(10, 30)))
                        .Select(async _ =>
                        {
                            var playerId = random.Next(1, 101);
                            var newPosition = new Vector3(
                                random.NextSingle() * 200 - 100,
                                0,
                                random.NextSingle() * 200 - 100
                            );

                            var moveRequest = new MoveRequest(playerId, newPosition, 0.033f, Interlocked.Increment(ref sequenceId));

                            // TPL Dataflow 파이프라인으로 이동 처리
                            await _pipeline.ProcessMoveAsync(moveRequest);

                            // 플레이어 액터로도 메시지 전송
                            await _updateStreamer.ProcessPlayerMoveAsync(moveRequest);
                        });

                    await Task.WhenAll(moveTasks);
                    await Task.Delay(33, _cancellationTokenSource.Token); // ~30 FPS
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Movement simulation error: {ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }

        private async Task StreamUpdatesAsync()
        {
            // 여러 플레이어의 시야 업데이트 스트림 동시 처리
            var streamTasks = Enumerable.Range(1, 10).Select(async playerId =>
            {
                await foreach (var visiblePlayers in _updateStreamer.GetVisibilityUpdatesAsync(playerId, _cancellationTokenSource.Token))
                {
                    if (visiblePlayers.Length > 0)
                    {
                        Console.WriteLine($"🔍 Player {playerId} sees {visiblePlayers.Length} other players");
                        _metrics.IncrementCounter("VisibilityUpdates");
                    }
                }
            });

            // Section 업데이트 스트림도 동시 처리
            var sectionStreamTasks = new[]
            {
                new SectionId(0, 0), new SectionId(1, 0), new SectionId(0, 1), new SectionId(1, 1)
            }
            .Select(async sectionId =>
            {
                await foreach (var batch in _updateStreamer.GetSectionUpdatesAsync(sectionId, _cancellationTokenSource.Token))
                {
                    if (batch.Events.Length > 0)
                    {
                        Console.WriteLine($"📦 Section {sectionId} batch: {batch.Events.Length} events");
                        _metrics.IncrementCounter("SectionUpdates");
                    }
                }
            });

            try
            {
                await Task.WhenAll(streamTasks.Concat(sectionStreamTasks));
            }
            catch (OperationCanceledException)
            {
                // 정상 종료
            }
        }

        private async Task RunCollisionDetectionAsync()
        {
            try
            {
                // 모든 활성 플레이어의 상태 수집
                var playerStates = new List<PlayerState>();

                for (int playerId = 1; playerId <= 100; playerId++)
                {
                    // 실제 구현에서는 플레이어 액터에서 상태를 가져옴
                    var position = new Vector3(
                        (float)(Math.Sin(DateTime.UtcNow.Millisecond / 1000.0 * Math.PI * 2 + playerId) * 50),
                        0,
                        (float)(Math.Cos(DateTime.UtcNow.Millisecond / 1000.0 * Math.PI * 2 + playerId) * 50)
                    );

                    var section = _spatialManager.GetSection(position);
                    var state = new PlayerState(playerId, position, Vector3.Zero, section, 100, 100, DateTime.UtcNow);
                    playerStates.Add(state);
                }

                // TPL Dataflow로 충돌 감지 처리
                await _pipeline.ProcessCollisionDetectionAsync(playerStates.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Collision detection error: {ex.Message}");
            }
        }

        private void PrintServerStats()
        {
            Console.WriteLine("\n=== 🎮 Server Statistics ===");
            Console.WriteLine($"🏃 Active Players: 100");
            Console.WriteLine($"🗺️  Active Sections: {_spatialManager.GetActiveSectionCount()}");
            Console.WriteLine($"⚡ Thread Pool: {ThreadPool.ThreadCount} threads");
            Console.WriteLine($"💾 Memory: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
            Console.WriteLine($"🔄 Generation 0 GC: {GC.CollectionCount(0)}");
            Console.WriteLine("=====================================\n");
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            // 모든 플레이어 제거
            for (int i = 1; i <= 100; i++)
            {
                _updateStreamer.RemovePlayer(i);
            }

            _pipeline?.Dispose();
            _eventManager?.Dispose();
            _metricsTimer?.Dispose();
            _collisionTimer?.Dispose();
            _cancellationTokenSource?.Dispose();
        }
    }
}
