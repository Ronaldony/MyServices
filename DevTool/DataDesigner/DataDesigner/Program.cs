using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace DataDesigner
{
    using DataDesigner.Core.CodeGenerator;
    using System.CodeDom.Compiler;

    /// <summary>
    /// Program.
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            // Set configuration file.
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddEnvironmentVariables()
                .Build();

            // Setup host.
            var host = Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(options =>
                {
                    // Configure host.
                    options.AddConfiguration(configRoot);
                    options.AddEnvironmentVariables();
                })
                .ConfigureLogging(options =>
                {
                    // Configure Logging.
                    options.ClearProviders();
                    options.AddNLog();
                    options.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<EnumCodeGenerator>(serviceProvider => new EnumCodeGenerator(serviceProvider));
                    services.AddSingleton<ClassCodeGenerator>(serviceProvider => new ClassCodeGenerator(serviceProvider));
                })
                .Build();

            var logger = LogManager
                .Setup()
                .SetupExtensions(ext => ext.RegisterConfigSettings(configRoot))
                .GetCurrentClassLogger();

            // Initialize services.
            var services = host.Services;

            // EnumCodeGenerator.
            var enumCodeGenerator = services.GetRequiredService<EnumCodeGenerator>();
            enumCodeGenerator.Initialize(AppDomain.CurrentDomain.BaseDirectory, "GeneratedEnum.dll");

            // EnumCodeGenerator.
            var classCodeGenerator = services.GetRequiredService<ClassCodeGenerator>();

            ////////////////////////////////////////////////
            /// Test.

            Test_EnumCodeGenerator(host.Services);
            Test_ClassCodeGenerator(host.Services);
        }

        /// <summary>
        /// Test for EnumCodeGenerator.
        /// </summary>
        private static void Test_EnumCodeGenerator(IServiceProvider serviceProvider)
        {
            ////////////////////////////////////////////////////////////////////
            /// Setup test datas.
            var codeDatas = new List<EnumCodeMember>();

            for (int cnt = 0; cnt < 10; ++cnt)
            {
                codeDatas.Add(new EnumCodeMember
                {
                    Name = $"Key{cnt}",
                    Value = cnt,
                    Comment = $"Comment{cnt}"
                });
            }
            ////////////////////////////////////////////////////////////////////
            var codeGenerator = serviceProvider.GetRequiredService<EnumCodeGenerator>();
            codeGenerator.AddType("TestCode", "Type_EnumCode", codeDatas);
            codeGenerator.AddType("TestCode", "Type_EnumCode2", codeDatas);

            codeGenerator.CreateFiles(AppDomain.CurrentDomain.BaseDirectory);
        }

        /// <summary>
        /// Test for ClassCodeGenerator.
        /// </summary>
        private static void Test_ClassCodeGenerator(IServiceProvider serviceProvider)
        {
            ////////////////////////////////////////////////////////////////////
            /// Setup test datas.
            var codeDatas = new List<ClassCodeMember>();

            for (int cnt = 0; cnt < 2; ++cnt)
            {
                // Int.
                codeDatas.Add(new ClassCodeMember
                {
                    Name = $"TestInt{cnt}",
                    Type = "int",
                    Comment = $"Comment - Int"
                });

                // string.
                codeDatas.Add(new ClassCodeMember
                {
                    Name = $"TestString{cnt}",
                    Type = "string",
                    Comment = $"Comment - string"
                });

                // double.
                codeDatas.Add(new ClassCodeMember
                {
                    Name = $"TestDouble{cnt}",
                    Type = "double",
                    Comment = $"Comment - double"
                });

                // bool.
                codeDatas.Add(new ClassCodeMember
                {
                    Name = $"TestBool{cnt}",
                    Type = "bool",
                    Comment = $"Comment - bool"
                });

                // Type_EnumCode.
                codeDatas.Add(new ClassCodeMember
                {
                    Name = $"TestEnumCode{cnt}",
                    Type = "Type_EnumCode",
                    Comment = $"Comment - Type_EnumCode"
                });

                // Type_EnumCode.
                codeDatas.Add(new ClassCodeMember
                {
                    Name = $"TestEnumCode_{cnt}",
                    Type = "Type_EnumCode2",
                    Comment = $"Comment - Type_EnumCode"
                });
            }

            ////////////////////////////////////////////////////////////////////
            var codeGenerator = serviceProvider.GetRequiredService<ClassCodeGenerator>();
            codeGenerator.Initialize(AppDomain.CurrentDomain.BaseDirectory, "GeneratedClass.dll");
            codeGenerator.AddClass(AppDomain.CurrentDomain.BaseDirectory, "TestCode", "ClassCode", codeDatas);
            codeGenerator.CreateFile();

            var types = codeGenerator.GetNewClassTypes();
        }
    }
}