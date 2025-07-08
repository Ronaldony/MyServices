using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks.Dataflow;

namespace TestProject.Datas
{
    /// <summary>
    // Actor 기반 Entity
    /// EntityActor.
    /// </summary>
    public class EntityActor
    {
        private readonly ActionBlock<Message> _messageBox;
        private EntityState _state;
        private readonly Subject<GameEvent> _eventStream;

        public Guid Id => _state.Id;
        public IObservable<GameEvent> Events => _eventStream.AsObservable();

        public EntityActor(EntityState initialState, IObserver<GameEvent> globalEventStream)
        {
            _state = initialState;
            _eventStream = new Subject<GameEvent>();
            _eventStream.Subscribe(globalEventStream);

            _messageBox = new ActionBlock<Message>(
                async message => await ProcessMessageAsync(message),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1, // 단일 스레드로 메시지 처리 (Actor 원칙)
                    BoundedCapacity = 1000
                });
        }

        private async Task ProcessMessageAsync(Message message)
        {
            switch (message)
            {
                case MoveMessage move:
                    if (_state.IsActive)
                    {
                        var oldPosition = _state.Position;
                        _state = _state.WithPosition(move.TargetPosition);

                        var moveEvent = new EntityMovedEvent(
                            _state.Id,
                            oldPosition,
                            move.TargetPosition
                        )
                        { SequenceNumber = _state.Version };

                        _eventStream.OnNext(moveEvent);
                    }
                    break;

                case GetStateMessage _:
                    // 상태 조회는 별도 채널로 응답
                    break;

                case DespawnMessage _:
                    if (_state.IsActive)
                    {
                        _state = _state.Deactivate();
                        var despawnEvent = new EntityDespawnedEvent(_state.Id)
                        {
                            SequenceNumber = _state.Version
                        };
                        _eventStream.OnNext(despawnEvent);
                    }
                    break;
            }

            await Task.CompletedTask;
        }

        public async Task<bool> SendMessageAsync(Message message)
        {
            return await _messageBox.SendAsync(message);
        }

        public EntityState GetState() => _state;

        public void Complete()
        {
            _messageBox.Complete();
            _eventStream.OnCompleted();
        }
    }
}
