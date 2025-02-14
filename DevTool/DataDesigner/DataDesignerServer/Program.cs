
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog.Extensions.Logging;
using System.Net;

namespace DataDesignerServerServer
{
    using DataDesignerServer;
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Filter;
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

			// Controllers.
			builder.Services.AddControllers(options =>
			{
				// Exception.
				options.Filters.Add<ErrorrExceptionFilter>();
			})
				// Newtonsoft
				.AddNewtonsoftJson(options =>
				{
					// Enum을 문자열로 변환
					options.SerializerSettings.Converters.Add(new StringEnumConverter());

					// Null 값을 JSON에서 제거
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

				});

			// Add services to the container.
			builder.Services.AddSingleton<CodeGenerator>();
			builder.Services.AddSingleton<DBGenerator>();

            builder.Services.AddSingleton<TypeDataManager>();
            builder.Services.AddSingleton<ClassDataManager>();

            builder.Services.AddSingleton<ProfileService>();
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