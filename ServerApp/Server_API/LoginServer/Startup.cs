using NLog.Extensions.Logging;
using ServerEngine.Core.Services;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.GeoIP;
using System.Net;

namespace LoginServer
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObjects;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.Core.WebAPI.Formatters;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using GameCore;
    using GameCore.Interfaces;

    public class Startup
    {
        public WebApplication WebApp;
        private WebApplicationBuilder _webAppBuilder;
        private readonly Arguments _arguments;

        public Startup(string[] args, Arguments arguments)
        {
            _arguments = arguments;

            // set builder.
            _webAppBuilder = WebApplication.CreateBuilder(args);
            _webAppBuilder.Configuration.AddJsonFile(_arguments.ConfigFile);
            _webAppBuilder.Configuration.SetBasePath(AppContext.BaseDirectory);
        }

        public void Configure()
        {
            // logging.
            _webAppBuilder.Logging.ClearProviders();
            _webAppBuilder.Logging.AddNLog();
            _webAppBuilder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

            // MVC
            _webAppBuilder.Services.AddMvcCore(option =>
            {
                option.InputFormatters.Clear();
                option.OutputFormatters.Clear();

                // memorypack formatter
                option.InputFormatters.Add(new BrotliMemorypackInputFormatter());
                option.OutputFormatters.Add(new BrotliMemorypackOutputFormatter());

                option.EnableEndpointRouting = false;
            })
            .AddApplicationPart(typeof(GameController).Assembly);

            _webAppBuilder.Services.AddControllers(options =>
            {
                options.Filters.Add<ErrorExceptionFilter>();
            });

            // services
            _webAppBuilder.Services.AddSingleton<IRemoteConfigureService, StrapiConfigureService>();
            _webAppBuilder.Services.AddSingleton<IUniqueIdService, SnowflakeService>();
            _webAppBuilder.Services.AddSingleton<IDataSerializer, MemoryPackDataSerializer>();
            _webAppBuilder.Services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();

            _webAppBuilder.Services.AddSingleton<IMemoryPoolService, CustomMemoryPoolService>();
            _webAppBuilder.Services.AddSingleton<IGeoIPService, MaxMindIPService>();
            _webAppBuilder.Services.AddSingleton<ITDBService, TDBService>();

            _webAppBuilder.Services.AddSingleton<DBInventoryInfo>();
            _webAppBuilder.Services.AddSingleton<DBPlayerInfo>();
            _webAppBuilder.Services.AddSingleton<DBPlayInfo>();
            _webAppBuilder.Services.AddSingleton<DBSessionInfo>();

            _webAppBuilder.Services.AddSingleton<DBIdentity>();
            _webAppBuilder.Services.AddSingleton<DBUser>();
            _webAppBuilder.Services.AddSingleton<DBPlayerData>();

            // Transient.
            _webAppBuilder.Services.AddTransient<AuthActionFilter>();

            _webAppBuilder.Services.AddTransient<InventoryInfoObject>();
            _webAppBuilder.Services.AddTransient<PlayerInfoObject>();
            _webAppBuilder.Services.AddTransient<PlayInfoObject>();
            _webAppBuilder.Services.AddTransient<SessionInfoObject>();

            // scoped.
            _webAppBuilder.WebHost.UseKestrel(configs =>
            {
                configs.Listen(IPAddress.Any, _arguments.Port);
            });

            // Build WebApplication.
            WebApp = _webAppBuilder.Build();

            WebApp.UseAuthorization();
            WebApp.MapControllers();
        }
    }
}
