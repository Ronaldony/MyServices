
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.DataManager.TableData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Generator;
    using DataDesignerServer.Core.Http.Generate;
    using DataDesignerServer.Core.Services;
    using DataDesignerServer.Core.Types;
    using System.Runtime;

    /// <summary>
    /// GenerateController .
    /// </summary>
    public class GenerateController : Controller
    {
        private const string GENERATE_NAMESPACE = "GameCore";

        private readonly TypeDataManager _typeDataManager;
        private readonly TableDataManager _tableDataManager;
        private readonly ConfigDataManager _configDataManager;

        private readonly CodeGenerator _codeGenerator;
        private readonly DBGenerator _dbGenerator;

        public GenerateController(
            TypeDataManager typeDataManager,
            TableDataManager tableDataManager,
            ConfigDataManager configDataManager,
            ProfileService profileService,
            CodeGenerator codeGenerator,
            DBGenerator dbGenerator)
        {
            _typeDataManager = typeDataManager;
            _tableDataManager = tableDataManager;
            _configDataManager = configDataManager;

            _codeGenerator = codeGenerator;
            _dbGenerator = dbGenerator;
        }

        [HttpPost]
        [Route("/generate/data")]
        public RES_Generate Process([FromBody] REQ_Generate req)
        {
            // check name
            if (string.IsNullOrEmpty(req.Profile.Name))
            {
                throw new ServerException(ServerErrorCode.Generate_NoProfileName, "no profile name");
            }

            // Check target.
            var filePath = req.Profile.FilePathList.FirstOrDefault(d => d.Target.Equals(req.Target));
            if (default == filePath)
            {
                throw new ServerException(ServerErrorCode.Generate_WrongTarget, "no target");
            }

            CreateFiles(filePath);

            ///////////////////////////////////////////////////////////
            // Generate Type.
            if (false == GenerateTypes(filePath.DatabaseFilePath))
            {
                throw new ServerException(ServerErrorCode.Generate_Data, "fail to generate type code");
            }

            // Generate Table.
            if (false == GenerateTables(filePath.DatabaseFilePath))
            {
                throw new ServerException(ServerErrorCode.Generate_Data, "fail to generate table code");
            }

            // Generate Table.
            if (false == GenerateContiruations(filePath.DatabaseFilePath))
            {
                throw new ServerException(ServerErrorCode.Generate_Data, "fail to generate configuration code");
            }

            // Data code.
            _codeGenerator.CreateFiles(filePath.DataCodePath);

            return new RES_Generate();
        }

        /// <summary>
        /// Create files.
        /// </summary>
        private void CreateFiles(Profile_FilePath filePath)
        {
            // Project path.
            if (false == Directory.Exists(filePath.ProjectPath))
            {
                Directory.CreateDirectory(filePath.ProjectPath);
            }

            // Protocol path.
            if (false == Directory.Exists(filePath.ProtocolPath))
            {
                Directory.CreateDirectory(filePath.ProtocolPath);
            }

            // Data code path.
            if (false == Directory.Exists(filePath.DataCodePath))
            {
                Directory.CreateDirectory(filePath.DataCodePath);
            }

            // Database path
            if (false == Directory.Exists(filePath.DatabaseFilePath))
            {
                Directory.CreateDirectory(filePath.DatabaseFilePath);
            }
        }

        /// <summary>
        /// Generate code for type data.
        /// </summary>
        private bool GenerateTypes(string dbFilePath)
        {
            // types.
            var types = _typeDataManager.GetAllType();
            foreach (var type in types)
            {
                // Generate Code.
                var schema = _typeDataManager.FindSchemaOrDefault(type.Name);
                var rows = _typeDataManager.GetRowsOrDefault(type.Name);

                _codeGenerator.AddEnum(Type_GenerateCode.Type, GENERATE_NAMESPACE, schema.Name, rows);

                // Generate Database.
                _dbGenerator.GenerateEnum(dbFilePath, type, rows);
            }

            return true;
        }

        /// <summary>
        /// Generate code for table data.
        /// </summary>
        private bool GenerateTables(string dbFilePath)
        {
            // Generate Type.
            var types = _tableDataManager.GetAllType();
            foreach (var type in types)
            {
                // Generate code.
                var schema = _tableDataManager.FindSchemaOrDefault(type.Name);

                _codeGenerator.AddClass(Type_GenerateCode.Table, GENERATE_NAMESPACE, schema.Name, schema.Columns);

                // Generate Database.
                var rows = _tableDataManager.FindRowsOrDefault(type.Name);

                _dbGenerator.GenerateClass(dbFilePath, type, schema, rows);
            }

            return true;
        }

        /// <summary>
        /// Generate code for configuration data.
        /// </summary>
        private bool GenerateContiruations(string dbFilePath)
        {
            // Generate Type.
            var types = _configDataManager.GetAllType();
            foreach (var type in types)
            {
                // Generate code.
                var schema = _configDataManager.FindSchemaOrDefault(type.Name);

                _codeGenerator.AddClass(Type_GenerateCode.Configuration, GENERATE_NAMESPACE, schema.Name, schema.Columns);

                // Generate Database.
                var rows = _configDataManager.FindRowsOrDefault(type.Name);

                _dbGenerator.GenerateClass(dbFilePath, type, schema, rows);
            }

            return true;
        }
    }
}
