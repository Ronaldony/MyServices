namespace DataDesignerServer
{
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.DataManager.TableData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Generator;
    using DataDesignerServer.Core.Services;

    public class StartupServer
    {
        private readonly IServiceProvider _serviceProvider;

        public StartupServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Startup()
        {
            // CodeGenerator.
            var codeGenerator = _serviceProvider.GetRequiredService<CodeGenerator>();
            codeGenerator.Initialize();

            // EnumManager.
            var enumManager = _serviceProvider.GetRequiredService<TypeDataManager>();
            enumManager.Initialize(Directory.GetCurrentDirectory());

            // ClassManager.
            var classManager = _serviceProvider.GetRequiredService<TableDataManager>();
            classManager.Initialize(Directory.GetCurrentDirectory());

            // ConfigDataManager.
            var configDataManager = _serviceProvider.GetRequiredService<ConfigDataManager>();
            configDataManager.Initialize(Directory.GetCurrentDirectory());

            // ProfileService.
            var profileService = _serviceProvider.GetRequiredService<ProfileService>();
            profileService.Initialize(Directory.GetCurrentDirectory());
        }
    }
}