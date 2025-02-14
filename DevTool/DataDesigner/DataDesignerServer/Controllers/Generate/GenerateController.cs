
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Generator;
    using DataDesignerServer.Core.Http.Generate;
    using DataDesignerServer.Core.Services;
    using DataDesignerServer.Core.Types;

    /// <summary>
    /// GenerateController .
    /// </summary>
    public class GenerateController : Controller
    {
        private const string GENERATE_NAMESPACE = "GameCore";

        private readonly ClassDataManager _classDataManager;
        private readonly TypeDataManager _typeDataManager;

        private readonly CodeGenerator _codeGenerator;
        private readonly DBGenerator _dbGenerator;

        public GenerateController(
            TypeDataManager typeDataManager,
            ClassDataManager classDataManager,
            ProfileService profileService,
            CodeGenerator codeGenerator,
            DBGenerator dbGenerator)
        {
            _typeDataManager = typeDataManager;
            _classDataManager = classDataManager;

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
            if (false == GenerateClass(filePath.DatabaseFilePath))
            {
                throw new ServerException(ServerErrorCode.Generate_Data, "fail to generate class code");
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
            var types = _typeDataManager.GetEnumTypes();
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
        private bool GenerateClass(string dbFilePath)
        {
            // Generate Type.
            var types = _classDataManager.GetAllClassType();
            foreach (var type in types)
            {
                // Generate code.
                var schema = _classDataManager.FindSchemaOrDefault(type.Name);

                // Get GenerateCodeType.
                var genCodeType = _classDataManager.GetClassDataType(type.Name) switch
                {
                    Type_ClassData.Table => Type_GenerateCode.Table,
                    Type_ClassData.Configuration => Type_GenerateCode.Configuration,
                    _ => Type_GenerateCode.None
                };

                if (Type_GenerateCode.None == genCodeType)
                {
                    return false;
                }

                _codeGenerator.AddClass(genCodeType, GENERATE_NAMESPACE, schema.Name, schema.Columns);

                // Generate Database.
                var rows = _classDataManager.FindRowsOrDefault(type.Name);

                _dbGenerator.GenerateClass(dbFilePath, type, schema, rows);
            }

            return true;
        }
    }
}
