using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading.Tasks.Dataflow;

namespace TestProject.Datas
{
    /// <summary>
    // Message Router (중앙 메시지 라우터)
    /// MessageRouter.
    /// </summary>
    public class MessageRouter
    {
        private readonly ConcurrentDictionary<Guid, EntityActor> _actors;
        private readonly TransformBlock<Message, Message> _routingPipeline;
        private readonly BroadcastBlock<GameEvent> _eventBroadcast;

        public MessageRouter()
        {
            _actors = new ConcurrentDictionary<Guid, EntityActor>();
            _eventBroadcast = new BroadcastBlock<GameEvent>(evt => evt);

            _routingPipeline = new TransformBlock<Message, Message>(
                async message => await RouteMessageAsync(message),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    BoundedCapacity = 10000
                });
        }

        private async Task<Message> RouteMessageAsync(Message message)
        {
            switch (message)
            {
                case MoveMessage move:
                    if (_actors.TryGetValue(move.EntityId, out var actor))
                    {
                        await actor.SendMessageAsync(move);
                    }
                    break;

                case SpawnMessage spawn:
                    await CreateEntityAsync(spawn);
                    break;

                case DespawnMessage despawn:
                    if (_actors.TryGetValue(despawn.EntityId, out var actorToDespawn))
                    {
                        await actorToDespawn.SendMessageAsync(despawn);
                        _actors.TryRemove(despawn.EntityId, out _);
                    }
                    break;
            }

            return message;
        }

        private async Task CreateEntityAsync(SpawnMessage spawn)
        {
            var entityId = Guid.NewGuid();
            var initialState = new EntityState
            {
                Id = entityId,
                Type = spawn.Type,
                Position = spawn.Position,
                CollisionRadius = spawn.Type == EntityType.Player ? 1.0f : 0.5f,
                Properties = spawn.Properties?.ToImmutableDictionary() ?? ImmutableDictionary<string, object>.Empty,
                IsActive = true,
                Version = 0
            };

            var actor = new EntityActor(initialState, _eventBroadcast.AsObserver());
            _actors.TryAdd(entityId, actor);

            var spawnEvent = new EntitySpawnedEvent(
                entityId,
                spawn.Type,
                spawn.Position,
                initialState.Properties
            )
            { SequenceNumber = 0 };

            _eventBroadcast.Post(spawnEvent);
            await Task.CompletedTask;
        }

        public async Task<bool> SendMessageAsync(Message message)
        {
            return await _routingPipeline.SendAsync(message);
        }

        public IObservable<GameEvent> GetEventStream() => _eventBroadcast.AsObservable();

        public EntityActor GetActor(Guid entityId)
        {
            return _actors.TryGetValue(entityId, out var actor) ? actor : null;
        }
    }
}
