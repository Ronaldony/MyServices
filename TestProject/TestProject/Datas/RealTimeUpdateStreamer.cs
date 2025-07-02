using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Channels;

namespace TestProject.Datas
{
    // ===== 7. 실시간 업데이트 스트리밍 (IAsyncEnumerable) =====
    /// <summary>
    /// RealTimeUpdateStreamer.
    /// </summary>
    public class RealTimeUpdateStreamer
    {
        private readonly ConcurrentDictionary<int, PlayerActor> _playerActors = new();
        private readonly SpatialManager _spatialManager;
        private readonly EventStreamManager _eventManager;

        public RealTimeUpdateStreamer(SpatialManager spatialManager, EventStreamManager eventManager)
        {
            _spatialManager = spatialManager;
            _eventManager = eventManager;
        }

        // 특정 플레이어의 시야 내 업데이트 스트림
        public async IAsyncEnumerable<PlayerState[]> GetVisibilityUpdatesAsync(
            int observerPlayerId,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var updateInterval = TimeSpan.FromMilliseconds(33); // ~30 FPS

            while (!cancellationToken.IsCancellationRequested)
            {
                if (_playerActors.TryGetValue(observerPlayerId, out var observer))
                {
                    var observerState = await observer.GetStateAsync();
                    var nearbyPlayerIds = _spatialManager.GetNearbyPlayers(observerState.Section, 2);

                    var visiblePlayers = new List<PlayerState>();
                    foreach (var playerId in nearbyPlayerIds)
                    {
                        if (playerId != observerPlayerId && _playerActors.TryGetValue(playerId, out var playerActor))
                        {
                            var state = await playerActor.GetStateAsync();
                            if (IsInViewDistance(observerState.Position, state.Position, 50.0f))
                            {
                                visiblePlayers.Add(state);
                            }
                        }
                    }

                    if (visiblePlayers.Any())
                    {
                        yield return visiblePlayers.ToArray();
                    }
                }

                await Task.Delay(updateInterval, cancellationToken);
            }
        }

        // Section별 실시간 업데이트 스트림
        public async IAsyncEnumerable<SectionUpdateBatch> GetSectionUpdatesAsync(
            SectionId sectionId,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var channel = Channel.CreateUnbounded<SectionUpdateBatch>();
            var writer = channel.Writer;

            // 이벤트 스트림을 채널로 변환
            var subscription = _eventManager.GetEventStream<PlayerMoveEvent>()
                .Where(evt => _spatialManager.GetSection(evt.To) == sectionId)
                .Buffer(TimeSpan.FromMilliseconds(16)) // ~60 FPS
                .Subscribe(async events =>
                {
                    if (events.Any())
                    {
                        var batch = new SectionUpdateBatch(sectionId, events.ToArray(), DateTime.UtcNow);
                        await writer.WriteAsync(batch, cancellationToken);
                    }
                });

            try
            {
                await foreach (var batch in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    yield return batch;
                }
            }
            finally
            {
                subscription.Dispose();
                writer.Complete();
            }
        }

        private static bool IsInViewDistance(Vector3 pos1, Vector3 pos2, float maxDistance)
        {
            return pos1.DistanceTo(pos2) <= maxDistance;
        }

        public void AddPlayer(int playerId, Vector3 spawnPosition)
        {
            var actor = new PlayerActor(playerId, spawnPosition);
            _playerActors.TryAdd(playerId, actor);
        }

        public void RemovePlayer(int playerId)
        {
            if (_playerActors.TryRemove(playerId, out var actor))
            {
                actor.Dispose();
            }
        }

        public async Task<bool> ProcessPlayerMoveAsync(MoveRequest request)
        {
            if (_playerActors.TryGetValue(request.PlayerId, out var actor))
            {
                return await actor.SendMessageAsync(request);
            }
            return false;
        }
    }
}
