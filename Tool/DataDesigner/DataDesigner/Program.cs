
using NLog.Extensions.Logging;
using System.Net;

namespace DataDesigner
{
    using DataDesigner.Core.Data;
    using DataDesigner.Core.Generator;
    using DataDesigner.Core.TypeManager;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Set configuration file.
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .Build();

            // WebApplicationBuilder.
            var builder = WebApplication.CreateBuilder(args);
            
            // Configuration.
            builder.Configuration.AddConfiguration(configRoot);
            //builder.Configuration.AddEnvironmentVariables();

            // Logging.
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSingleton<CodeGenerator>(serviceProvider => new CodeGenerator(serviceProvider));
            builder.Services.AddSingleton<EnumManager>(serviceProvider => new EnumManager(serviceProvider));
            builder.Services.AddSingleton<ClassManager>(serviceProvider => new ClassManager(serviceProvider));

            // Web host.
            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(IPAddress.Any, 8010);
            });

            // builder.
            var app = builder.Build();
            app.MapControllers();

            // Startup Server.
            var startupServer = new StartupServer(app.Services);
            startupServer.Startup();

            app.Run();
        }
    }
}