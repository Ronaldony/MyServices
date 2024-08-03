using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Services;
using ServerEngine.Test.Database.DataObject;
using ServerEngine.Database.Interfaces;
using ServerEngine.Database.PostgreSQL.Services;

namespace ServerEngine.Test
{
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
            // services
            _webAppBuilder.Services.AddControllers();

            _webAppBuilder.Services.AddSingleton<IRemoteConfigureService, ConsulConfigureService>();
            _webAppBuilder.Services.AddSingleton<IUniqueIdService, SnowflakeService>();
            _webAppBuilder.Services.AddSingleton<IDataSerializer, MemoryPackDataSerializer>();
            _webAppBuilder.Services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
            _webAppBuilder.Services.AddSingleton<IDataObjectService, DataObjectService_PSQL>();

            // Database.
            _webAppBuilder.Services.AddScoped<PlayerInfoObejct>();

            _webAppBuilder.WebHost.ConfigureKestrel(d =>
            {
                d.ListenAnyIP(_arguments.Port);
            });

            // Build WebApplication.
            WebApp = _webAppBuilder.Build();

            //WebApp.UseHttpsRedirection();
            WebApp.UseAuthorization();
            WebApp.MapControllers();
        }
    }
}
