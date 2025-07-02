using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TestProject.Datas
{
    // ===== 6. 이벤트 스트림 관리자 (Reactive Extensions) =====
    /// <summary>
    /// EventStreamManager.
    /// </summary>
    public class EventStreamManager : IDisposable
    {
        private readonly Subject<GameEvent> _gameEventSubject = new();
        private readonly Dictionary<Type, Subject<GameEvent>> _eventTypeSubjects = new();
        private readonly ConcurrentDictionary<int, Subject<GameEvent>> _playerEventSubjects = new();
        private readonly CompositeDisposable _subscriptions = new();

        public EventStreamManager()
        {
            // 이벤트 타입별 필터링
            _subscriptions.Add(
                _gameEventSubject
                    .GroupBy(evt => evt.GetType())
                    .Subscribe(group =>
                    {
                        var eventType = group.Key;
                        if (!_eventTypeSubjects.ContainsKey(eventType))
                        {
                            _eventTypeSubjects[eventType] = new Subject<GameEvent>();
                        }

                        group.Subscribe(_eventTypeSubjects[eventType]);
                    })
            );

            // 플레이어별 이벤트 스트림
            _subscriptions.Add(
                _gameEventSubject
                    .GroupBy(evt => evt.PlayerId)
                    .Subscribe(group =>
                    {
                        var playerId = group.Key;
                        var playerSubject = _playerEventSubjects.GetOrAdd(playerId, _ => new Subject<GameEvent>());
                        group.Subscribe(playerSubject);
                    })
            );
        }

        public void PublishEvent(GameEvent gameEvent)
        {
            _gameEventSubject.OnNext(gameEvent);
        }

        public IObservable<T> GetEventStream<T>() where T : GameEvent
        {
            var eventType = typeof(T);
            if (!_eventTypeSubjects.ContainsKey(eventType))
            {
                _eventTypeSubjects[eventType] = new Subject<GameEvent>();
            }

            return _eventTypeSubjects[eventType].OfType<T>();
        }

        public IObservable<GameEvent> GetPlayerEventStream(int playerId)
        {
            var subject = _playerEventSubjects.GetOrAdd(playerId, _ => new Subject<GameEvent>());
            return subject.AsObservable();
        }

        // 복잡한 이벤트 패턴 감지
        public IObservable<PlayerMoveEvent> GetSpeedingPlayers()
        {
            return GetEventStream<PlayerMoveEvent>()
                .Buffer(TimeSpan.FromSeconds(1))
                .SelectMany(moves => moves
                    .GroupBy(m => m.PlayerId)
                    .Where(g => g.Count() > 10) // 1초에 10번 이상 이동
                    .SelectMany(g => g));
        }

        public IObservable<CollisionEvent> GetFrequentCollisions()
        {
            return GetEventStream<CollisionEvent>()
                .Buffer(TimeSpan.FromSeconds(5))
                .SelectMany(collisions => collisions
                    .GroupBy(c => new { Player1 = Math.Min(c.PlayerId, c.OtherPlayerId), Player2 = Math.Max(c.PlayerId, c.OtherPlayerId) })
                    .Where(g => g.Count() > 3) // 5초에 3번 이상 충돌
                    .SelectMany(g => g));
        }

        public void Dispose()
        {
            _subscriptions?.Dispose();
            _gameEventSubject?.Dispose();
            foreach (var subject in _eventTypeSubjects.Values)
            {
                subject?.Dispose();
            }
            foreach (var subject in _playerEventSubjects.Values)
            {
                subject?.Dispose();
            }
        }
    }
}
