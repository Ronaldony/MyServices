
using DataDesigner.Core.Data;
using DataDesigner.Core.DataManager;
using DataDesigner.Core.Generator;
using NLog.Extensions.Logging;
using System.Net;

namespace DataDesigner
{
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

            /// Test.
            Test(app.Services);

            app.Run();
        }


        private static void Test(IServiceProvider serviceProvider)
        {
            var codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
            codeGenerator.Initialize();

            Test_EnumCodeGenerator(serviceProvider);
            Test_ClassCodeGenerator(serviceProvider);

            codeGenerator.CreateFiles(AppDomain.CurrentDomain.BaseDirectory);
        }

        /// <summary>
        /// Test for EnumCodeGenerator.
        /// </summary>
        private static void Test_EnumCodeGenerator(IServiceProvider serviceProvider)
        {
            ////////////////////////////////////////////////////////////////////
            /// Setup test datas.
            var codeDatas = new List<EnumMember>();

            for (int cnt = 0; cnt < 10; ++cnt)
            {
                codeDatas.Add(new EnumMember
                {
                    Name = $"Key{cnt}",
                    Value = cnt,
                    Comment = $"Comment{cnt}"
                });
            }
            ////////////////////////////////////////////////////////////////////
            var codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
            codeGenerator.AddEnum("TestCode", "Type_EnumCode", codeDatas);
            codeGenerator.AddEnum("TestCode", "Type_EnumCode2", codeDatas);
        }

        /// <summary>
        /// Test for ClassCodeGenerator.
        /// </summary>
        private static void Test_ClassCodeGenerator(IServiceProvider serviceProvider)
        {
            ////////////////////////////////////////////////////////////////////
            /// Setup test datas.
            var codeDatas = new List<ClassMember>();

            for (int cnt = 0; cnt < 2; ++cnt)
            {
                // Int.
                codeDatas.Add(new ClassMember
                {
                    Name = $"TestInt{cnt}",
                    Type = "int",
                    Comment = $"Comment - Int"
                });

                // string.
                codeDatas.Add(new ClassMember
                {
                    Name = $"TestString{cnt}",
                    Type = "string",
                    Comment = $"Comment - string"
                });

                // double.
                codeDatas.Add(new ClassMember
                {
                    Name = $"TestDouble{cnt}",
                    Type = "double",
                    Comment = $"Comment - double"
                });

                // bool.
                codeDatas.Add(new ClassMember
                {
                    Name = $"TestBool{cnt}",
                    Type = "bool",
                    Comment = $"Comment - bool"
                });

                // Type_EnumCode.
                codeDatas.Add(new ClassMember
                {
                    Name = $"TestEnumCode{cnt}",
                    Type = "Type_EnumCode",
                    Comment = $"Comment - Type_EnumCode"
                });

                // Type_EnumCode.
                codeDatas.Add(new ClassMember
                {
                    Name = $"TestEnumCode_{cnt}",
                    Type = "Type_EnumCode2",
                    Comment = $"Comment - Type_EnumCode"
                });
            }

            ////////////////////////////////////////////////////////////////////
            var codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
            codeGenerator.AddClass("TestCode", "ClassCode", codeDatas);
        }
    }
}