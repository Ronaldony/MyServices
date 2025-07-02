using TestProject.Datas;

// ===== 12. 메인 프로그램 및 시연 =====

namespace TestProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🎮 Advanced MMO Game Server Demo");
            Console.WriteLine("=================================");
            Console.WriteLine("This demo showcases:");
            Console.WriteLine("• Task-based async processing");
            Console.WriteLine("• IObservable event streams");
            Console.WriteLine("• IAsyncEnumerable real-time updates");
            Console.WriteLine("• TPL Dataflow pipelines");
            Console.WriteLine("• Actor pattern for players");
            Console.WriteLine("• Spatial partitioning");
            Console.WriteLine("• Reactive pattern detection");
            Console.WriteLine("=================================\n");

            using var server = new AdvancedGameServer();

            try
            {
                await server.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Server error: {ex.Message}");
            }

            Console.WriteLine("✅ Server stopped gracefully");
        }
    }
}