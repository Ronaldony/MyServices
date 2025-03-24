using NLog.Extensions.Logging;
using System.Net;

namespace LoginServer
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObject;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.Core.WebAPI.Formatters;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using GameCore;
    using GameCore.Interfaces;
    using Google.Api;
    using ServerEngine.Core.Services;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.GeoIP;

    public class Startup
    {
        public WebApplication WebApp { get; set; }

        private readonly Arguments _arguments;
        private WebApplicationBuilder _webAppBuilder;

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

            // Data objects.
            _webAppBuilder.Services.AddSingleton<SessionInfoObject>();
            _webAppBuilder.Services.AddSingleton<PlayerInfoObject>();
            _webAppBuilder.Services.AddSingleton<PlayInfoObject>();
            _webAppBuilder.Services.AddSingleton<InventoryInfoObject>();

            // Scoped
            _webAppBuilder.Services.AddScoped<DBIdentity>();
            _webAppBuilder.Services.AddScoped<DBUser>();
            _webAppBuilder.Services.AddScoped<DBPlayerData>();
            _webAppBuilder.Services.AddScoped<AuthActionFilter>();

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
