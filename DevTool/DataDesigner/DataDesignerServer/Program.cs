
using NLog.Extensions.Logging;
using System.Net;

namespace DataDesignerServerServer
{
    using DataDesignerServer;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.DataManager.TableData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Generator;
    using DataDesignerServer.Core.Services;
    using DataDesignerServer.Core.Services.Interfaces;

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
            builder.Services.AddSingleton<TypeDataManager>(serviceProvider => new TypeDataManager(serviceProvider));
            builder.Services.AddSingleton<ConfigDataManager>(serviceProvider => new ConfigDataManager(serviceProvider));
            builder.Services.AddSingleton<TableDataManager>(serviceProvider => new TableDataManager(serviceProvider));
            builder.Services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();

            // Web host.
            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(IPAddress.Any, 7000);
            });

            // builder.
            var app = builder.Build();
            app.MapControllers();
            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
            });

            // Startup Server.
            var startupServer = new StartupServer(app.Services);
            startupServer.Startup();

            app.Run();
        }
    }
}