using System.Threading.Channels;

namespace TestProject.Datas
{
    // ===== 4. 플레이어 액터 (Actor Pattern) =====
    /// <summary>
    /// PlayerActor.
    /// </summary>
    public class PlayerActor
    {
        private readonly int _playerId;
        private readonly Channel<object> _messageChannel;
        private readonly ChannelWriter<object> _messageWriter;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private PlayerState _currentState;
        private readonly SemaphoreSlim _stateLock = new(1, 1);

        public PlayerActor(int playerId, Vector3 spawnPosition)
        {
            _playerId = playerId;
            var options = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };
            _messageChannel = Channel.CreateBounded<object>(options);
            _messageWriter = _messageChannel.Writer;

            _currentState = new PlayerState(
                playerId, spawnPosition, Vector3.Zero,
                new SpatialManager().GetSection(spawnPosition), 100, 100, DateTime.UtcNow
            );

            // 메시지 처리 루프 시작
            _ = Task.Run(ProcessMessagesAsync);
        }

        public async Task<bool> SendMessageAsync(object message)
        {
            return await _messageWriter.WaitToWriteAsync(_cancellationTokenSource.Token) &&
                   _messageWriter.TryWrite(message);
        }

        public async Task<PlayerState> GetStateAsync()
        {
            await _stateLock.WaitAsync();
            try { return _currentState; }
            finally { _stateLock.Release(); }
        }

        private async Task ProcessMessagesAsync()
        {
            await foreach (var message in _messageChannel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
            {
                try
                {
                    await ProcessMessageAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Player {_playerId} message processing error: {ex.Message}");
                }
            }
        }

        private async Task ProcessMessageAsync(object message)
        {
            await _stateLock.WaitAsync();
            try
            {
                switch (message)
                {
                    case MoveRequest moveReq:
                        await HandleMoveRequestAsync(moveReq);
                        break;
                    case CollisionEvent collision:
                        await HandleCollisionAsync(collision);
                        break;
                    default:
                        Console.WriteLine($"Unhandled message type: {message.GetType()}");
                        break;
                }
            }
            finally
            {
                _stateLock.Release();
            }
        }

        private async Task HandleMoveRequestAsync(MoveRequest request)
        {
            var oldPosition = _currentState.Position;
            var newPosition = request.TargetPosition;

            // 이동 검증
            var distance = oldPosition.DistanceTo(newPosition);
            var maxDistance = PlayerState.MaxSpeed * request.DeltaTime;

            if (distance > maxDistance * 1.1f) // 10% 허용 오차
            {
                // 이동 거부
                return;
            }

            // 상태 업데이트
            var newSection = new SpatialManager().GetSection(newPosition);
            _currentState = _currentState with
            {
                Position = newPosition,
                Section = newSection,
                LastUpdate = DateTime.UtcNow
            };

            await Task.Delay(1); // 비동기 처리 시뮬레이션
        }

        private async Task HandleCollisionAsync(CollisionEvent collision)
        {
            // 충돌 처리 로직
            Console.WriteLine($"Player {_playerId} collision with {collision.OtherPlayerId}");
            await Task.Delay(1);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _messageWriter.Complete();
            _stateLock?.Dispose();
            _cancellationTokenSource?.Dispose();
        }
    }
}
