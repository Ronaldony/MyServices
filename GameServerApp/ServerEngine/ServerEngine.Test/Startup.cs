﻿using Microsoft.Extensions.ObjectPool;
using System.Net;

namespace ServerEngine.Test
{
    using ServerEngine.Core.Services;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Cache;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.GeoIP;
    using ServerEngine.Test.Database.DataObject;

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
            _webAppBuilder.Services.AddControllers();

            // cache.
            //_webAppBuilder.Services.AddEnyimMemcached(options =>
            //{
            //    options.AddServer("192.168.10.6", 11211);
            //});

            // services
            _webAppBuilder.Services.AddSingleton<IRemoteConfigureService, ConsulConfigureService>();
            _webAppBuilder.Services.AddSingleton<IUniqueIdService, SnowflakeService>();
            _webAppBuilder.Services.AddSingleton<IDataSerializer, MemoryPackDataSerializer>();
            _webAppBuilder.Services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
            _webAppBuilder.Services.AddSingleton<IMemcachedService, MemcachedService>();

            _webAppBuilder.Services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            _webAppBuilder.Services.AddSingleton<IObjectPoolService, DataObjectPoolService>();
            _webAppBuilder.Services.AddSingleton<IGeoIPService, MaxMindIPService>();

			// scoped.
			_webAppBuilder.Services.AddScoped<PlayerInfoObejct>();

			_webAppBuilder.WebHost.UseKestrel(configs =>
            {
                configs.Listen(IPAddress.Any, _arguments.Port);
            });

            // Build WebApplication.
            WebApp = _webAppBuilder.Build();

            //WebApp.UseHttpsRedirection();
            WebApp.UseAuthorization();
            //WebApp.UseEnyimMemcached();
            WebApp.MapControllers();
        }
    }
}
