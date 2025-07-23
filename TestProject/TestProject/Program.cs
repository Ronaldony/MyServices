using TestProject.Creatures;
using TestProject.Servers;
using TestProject.Types;

namespace MMORPGServer.AI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Create and start the server
            var server = new GameServer();
            await server.RunServer(); // Handles 500 monsters + 1500 players

            // Create custom creature
            var template = new CreatureTemplate
            {
                Name = "Elite Orc",
                AIType = AIType.Aggressive,
                AggroRange = 20f,
                AttackRange = 3f,
                MaxHealth = 500f
            };

            //var creature = new Creature(entityId, template, spawnPosition);
            //WorldManager.Instance.AddObject(creature);
        }
    }
}