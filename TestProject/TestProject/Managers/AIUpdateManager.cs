using TestProject.Objects;

namespace TestProject.Managers
{
    /// <summary>
    /// AIUpdateManager.
    /// </summary>
    public class AIUpdateManager
    {
        private readonly List<Creature> _creatures = new();
        private readonly object _lock = new object();

        // Parallel processing options
        private readonly ParallelOptions _parallelOptions;

        public AIUpdateManager()
        {
            _parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 1
            };
        }

        public void RegisterCreature(Creature creature)
        {
            lock (_lock)
            {
                _creatures.Add(creature);
            }
        }

        public void UnregisterCreature(Creature creature)
        {
            lock (_lock)
            {
                _creatures.Remove(creature);
            }
        }

        public async Task UpdateAllCreatures(float deltaTime)
        {
            List<Creature> creaturesSnapshot;

            lock (_lock)
            {
                creaturesSnapshot = new List<Creature>(_creatures);
            }

            // Parallel update for performance with 2000+ creatures
            await Task.Run(() =>
            {
                Parallel.ForEach(creaturesSnapshot, _parallelOptions, creature =>
                {
                    try
                    {
                        creature.Update(deltaTime);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't crash other creatures
                        Console.WriteLine($"Error updating creature {creature.Id}: {ex.Message}");
                    }
                });
            });
        }
    }
}
