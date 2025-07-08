using System.Threading.Tasks.Dataflow;

namespace TestProject.Datas
{
    /// <summary>
    // Collision Detection System (Lock-free)
    /// CollisionDetectionSystem.
    /// </summary>
    public class CollisionDetectionSystem
    {
        private readonly LockFreeGridSystem _gridSystem;
        private readonly MessageRouter _router;
        private readonly ActionBlock<CollisionCheckRequest> _collisionPipeline;

        private record CollisionCheckRequest(Guid EntityId, Vector3 Position, float Radius);

        public CollisionDetectionSystem(LockFreeGridSystem gridSystem, MessageRouter router)
        {
            _gridSystem = gridSystem;
            _router = router;

            _collisionPipeline = new ActionBlock<CollisionCheckRequest>(
                async request => await CheckCollisionsAsync(request),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    BoundedCapacity = 1000
                });
        }

        private async Task CheckCollisionsAsync(CollisionCheckRequest request)
        {
            var nearbyEntities = new List<Guid>();

            await foreach (var entityId in _gridSystem.GetNearbyEntitiesAsync(request.Position, request.Radius * 2))
            {
                if (entityId != request.EntityId)
                {
                    nearbyEntities.Add(entityId);
                }
            }

            // 충돌 검사
            foreach (var otherEntityId in nearbyEntities)
            {
                var otherActor = _router.GetActor(otherEntityId);
                if (otherActor != null)
                {
                    var otherState = otherActor.GetState();
                    if (otherState.IsActive)
                    {
                        var distance = request.Position.DistanceTo(otherState.Position);
                        var collisionDistance = request.Radius + otherState.CollisionRadius;

                        if (distance < collisionDistance)
                        {
                            var collisionPoint = new Vector3(
                                (request.Position.X + otherState.Position.X) / 2,
                                (request.Position.Y + otherState.Position.Y) / 2,
                                (request.Position.Z + otherState.Position.Z) / 2
                            );

                            var collisionMessage = new CollisionDetectedMessage(
                                request.EntityId,
                                otherEntityId,
                                collisionPoint
                            );

                            // 충돌 이벤트 발생 (별도 처리)
                            Console.WriteLine($"Collision: {request.EntityId} <-> {otherEntityId}");
                        }
                    }
                }
            }
        }

        public async Task CheckCollisionAsync(Guid entityId, Vector3 position, float radius)
        {
            await _collisionPipeline.SendAsync(new CollisionCheckRequest(entityId, position, radius));
        }
    }
}
