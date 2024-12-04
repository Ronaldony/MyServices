
namespace DataDesigner
{
    using DataDesigner.Core.Parser;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Program.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // host.
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                })
                .Build();


            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // TODO: Load datas.

            // TODO: Parse datas.
            //var enumParser = new EnumParser();




            // TODO: Upload datas.
        }
    }
}