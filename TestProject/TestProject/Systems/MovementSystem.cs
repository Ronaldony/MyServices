using System.Numerics;
using TestProject.Managers;
using TestProject.Objects;

namespace TestProject.Systems
{
    /// <summary>
    /// MovementSystem.
    /// </summary>
    public class MovementSystem
    {
        private readonly Creature _owner;

        public float MovementSpeed { get; set; } = 5f;
        public float RunSpeed { get; set; } = 8f;
        public bool IsMoving { get; private set; }
        public Vector3 Destination { get; private set; }

        private readonly Queue<Vector3> _pathQueue = new();

        public MovementSystem(Creature owner)
        {
            _owner = owner;
        }

        public void Update(float deltaTime)
        {
            if (!IsMoving) return;

            var currentPos = _owner.Position;
            var direction = Vector3.Normalize(Destination - currentPos);
            var distance = Vector3.Distance(currentPos, Destination);

            if (distance < 0.5f)
            {
                // Reached destination
                _owner.Position = Destination;

                if (_pathQueue.Count > 0)
                {
                    Destination = _pathQueue.Dequeue();
                }
                else
                {
                    Stop();
                }
                return;
            }

            // Move towards destination
            var movement = direction * MovementSpeed * deltaTime;
            _owner.Position = currentPos + movement;

            // Update in world systems
            WorldManager.Instance.UpdateObjectPosition(_owner, currentPos, _owner.Position);
        }

        public void MoveTowards(Vector3 destination)
        {
            Destination = destination;
            IsMoving = true;
        }

        public void SetPath(List<Vector3> path)
        {
            _pathQueue.Clear();
            foreach (var point in path)
            {
                _pathQueue.Enqueue(point);
            }

            if (_pathQueue.Count > 0)
            {
                Destination = _pathQueue.Dequeue();
                IsMoving = true;
            }
        }

        public void Stop()
        {
            IsMoving = false;
            _pathQueue.Clear();
        }

        public void FaceTarget(IWorldObject target)
        {
            var direction = Vector3.Normalize(target.Position - _owner.Position);
            // In real implementation, would update rotation smoothly
        }
    }
}
