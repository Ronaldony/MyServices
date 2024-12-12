using DataDesigner.Core.CodeGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DataDesigner
{

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
                    services.AddSingleton<EnumCodeGenerator>(serviceProvider => new EnumCodeGenerator(serviceProvider, "Generated.dll"));
                    services.AddSingleton<ClassCodeGenerator>(serviceProvider => new ClassCodeGenerator(serviceProvider));
                })
                .Build();

            var logger = LogManager
                .Setup()
                .SetupExtensions(ext => ext.RegisterConfigSettings(configRoot))
                .GetCurrentClassLogger();

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
            var codeDatas = new List<EnumCodeData>();

            for (int cnt = 0; cnt < 10; ++cnt)
            {
                codeDatas.Add(new EnumCodeData
                {
                    Key = $"Key{cnt}",
                    Value = cnt,
                    Comment = $"Comment{cnt}"
                });
            }

            ////////////////////////////////////////////////////////////////////
            var codeGenerator = serviceProvider.GetRequiredService<EnumCodeGenerator>();
            codeGenerator.Generate(AppDomain.CurrentDomain.BaseDirectory, "TestCode", "Type_EnumCode", codeDatas);
        }

        /// <summary>
        /// Test for ClassCodeGenerator.
        /// </summary>
        private static void Test_ClassCodeGenerator(IServiceProvider serviceProvider)
        {
            ////////////////////////////////////////////////////////////////////
            /// Setup test datas.
            var codeDatas = new List<ClassCodeData>();

            for (int cnt = 0; cnt < 2; ++cnt)
            {
                // Int.
                codeDatas.Add(new ClassCodeData
                {
                    Name = $"TestInt{cnt}",
                    Type = "int",
                    Comment = $"Comment - Int"
                });

                // string.
                codeDatas.Add(new ClassCodeData
                {
                    Name = $"TestString{cnt}",
                    Type = "string",
                    Comment = $"Comment - string"
                });

                // double.
                codeDatas.Add(new ClassCodeData
                {
                    Name = $"TestDouble{cnt}",
                    Type = "double",
                    Comment = $"Comment - double"
                });

                // bool.
                codeDatas.Add(new ClassCodeData
                {
                    Name = $"TestBool{cnt}",
                    Type = "bool",
                    Comment = $"Comment - bool"
                });

                // Type_EnumCode.
                codeDatas.Add(new ClassCodeData
                {
                    Name = $"TestEnumCode{cnt}",
                    Type = "Type_EnumCode",
                    Comment = $"Comment - Type_EnumCode"
                });
            }

            ////////////////////////////////////////////////////////////////////
            var codeGenerator = serviceProvider.GetRequiredService<ClassCodeGenerator>();
            codeGenerator.Initialize();
            codeGenerator.Generate(AppDomain.CurrentDomain.BaseDirectory, "TestCode", "ClassCode", codeDatas);
            var types = codeGenerator.GetGeneratedTypes();
        }
    }
}