using DataDesigner.Core.Generator;
using DataDesigner.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;

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
                .Build();

            var logger = LogManager
                .Setup()
                .SetupExtensions(ext => ext.RegisterConfigSettings(configRoot))
                .GetCurrentClassLogger();

            TestParse(host.Services);
        }

        private static void TestParse(IServiceProvider serviceProvider)
        {
            ////////////////////////////////////////////////////////////////////
            /// Setup test datas.
            var testCount = 3;
            var testClasses = new List<TestClass>();

            for (var cnt = 0; cnt < testCount; cnt++)
            {
                testClasses.Add(new TestClass
                {
                    TestInt = cnt,
                    TestFloat = cnt,
                    TestDouble = cnt,
                    TestString = cnt.ToString()
                });
            }

            // Json test data.
            var classJson = JsonConvert.SerializeObject(testClasses);
            var enumJson = JsonConvert.SerializeObject(Enum.GetValues<Type_Test>());

            var testEnumType = "Type_Program";
            var testEnums = new List<EnumData>();

            testEnums.Add(new EnumData
            {
                Key = "None",
                Value = 0,
                Comments = "First"
            });

            testEnums.Add(new EnumData
            {
                Key = "First",
                Value = 1,
                Comments = "Comment - first"
            });

            testEnums.Add(new EnumData
            {
                Key = "Second",
                Value = 3,
                Comments = "Comment - second"
            });

            testEnums.Add(new EnumData
            {
                Key = "Third",
                Value = 4,
                Comments = "Comment - third"
            });

            ////////////////////////////////////////////////////////////////////
            /// Parser.
            var enumParser = new EnumCodeGenerator(serviceProvider);
            enumParser.Generate("Program", testEnumType, testEnums);
        }
    }
}