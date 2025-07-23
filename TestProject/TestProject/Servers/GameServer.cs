using System.Numerics;
using TestProject.Creatures;
using TestProject.Entity;
using TestProject.Managers;
using TestProject.Objects;
using TestProject.Types;

namespace TestProject.Servers
{
    /// <summary>
    /// GameServer.
    /// </summary>
    public class GameServer
    {
        private readonly AIUpdateManager _aiManager;
        private readonly List<CreatureTemplate> _creatureTemplates;
        private uint _nextEntityId = 1;

        public GameServer()
        {
            _aiManager = new AIUpdateManager();
            _creatureTemplates = CreateDefaultTemplates();
        }

        public async Task RunServer()
        {
            // Create test world with creatures
            CreateTestWorld();

            // Main game loop
            var lastTime = DateTime.UtcNow;

            while (true)
            {
                var currentTime = DateTime.UtcNow;
                var deltaTime = (float)(currentTime - lastTime).TotalSeconds;
                lastTime = currentTime;

                // Update all AI creatures
                await _aiManager.UpdateAllCreatures(deltaTime);

                // Target 60 FPS
                await Task.Delay(16);
            }
        }

        private void CreateTestWorld()
        {
            // Create 500 monsters and 1500 players for testing
            var random = new Random();

            // Create creatures
            for (int i = 0; i < 500; i++)
            {
                var template = _creatureTemplates[random.Next(_creatureTemplates.Count)];
                var spawnPos = new Vector3(
                    random.Next(-500, 500),
                    0,
                    random.Next(-500, 500)
                );

                // 크리처 생성.
                var creature = new Creature(new EntityId(_nextEntityId++), template, spawnPos);

                // 크리처 오브젝트 추가.
                WorldManager.Instance.AddObject(creature);
                _aiManager.RegisterCreature(creature);
            }

            // Create players
            for (int i = 0; i < 1500; i++)
            {
                var player = new Player(new EntityId(_nextEntityId++))
                {
                    Position = new Vector3(
                        random.Next(-500, 500),
                        0,
                        random.Next(-500, 500)
                    )
                };

                WorldManager.Instance.AddObject(player);
            }

            Console.WriteLine("Test world created with 500 creatures and 1500 players!");
        }

        private List<CreatureTemplate> CreateDefaultTemplates()
        {
            return new List<CreatureTemplate>
            {
                new CreatureTemplate
                {
                    Id = 1,
                    Name = "Orc Warrior",
                    Level = 10,
                    MaxHealth = 150,
                    AIType = AIType.Aggressive,
                    SightRange = 20f,
                    AggroRange = 12f,
                    AttackRange = 2f,
                    AttackSpeed = 1.5f,
                    BaseDamage = 30f,
                    MovementSpeed = 6f
                },
                new CreatureTemplate
                {
                    Id = 2,
                    Name = "Forest Spider",
                    Level = 5,
                    MaxHealth = 80,
                    AIType = AIType.Defensive,
                    SightRange = 15f,
                    AggroRange = 8f,
                    AttackRange = 1.5f,
                    AttackSpeed = 2f,
                    BaseDamage = 20f,
                    MovementSpeed = 8f
                },
                new CreatureTemplate
                {
                    Id = 3,
                    Name = "Village Guard",
                    Level = 15,
                    MaxHealth = 250,
                    AIType = AIType.Guard,
                    SightRange = 30f,
                    AggroRange = 15f,
                    AttackRange = 2.5f,
                    AttackSpeed = 1.2f,
                    BaseDamage = 40f,
                    MovementSpeed = 5f,
                    LeashRange = 25f
                }
            };
        }
    }
}
