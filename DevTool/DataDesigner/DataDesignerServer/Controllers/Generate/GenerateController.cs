
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.DataManager.TableData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Generator;
    using DataDesignerServer.Core.Http.Generate;
    using DataDesignerServer.Core.Services;

    /// <summary>
    /// GenerateController .
    /// </summary>
    public class GenerateController : Controller
    {
        private const string GENERATE_NAMESPACE = "GameCore";

        private readonly TypeDataManager _typeDataManager;
        private readonly TableDataManager _tableDataManager;
        private readonly ConfigDataManager _configDataManager;

        private readonly ProfileService _profileService;
        private readonly CodeGenerator _codeGenerator;

        public GenerateController(
            TypeDataManager typeDataManager,
            TableDataManager tableDataManager,
            ConfigDataManager configDataManager,
            ProfileService profileService,
            CodeGenerator codeGenerator)
        {
            _typeDataManager = typeDataManager;
            _tableDataManager = tableDataManager;
            _configDataManager = configDataManager;

            _profileService = profileService;
            _codeGenerator = codeGenerator;
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

            // Update profile.
            if (false == _profileService.UpdateProfile(req.Profile))
            {
                throw new ServerException(ServerErrorCode.Generate_UpdateProfile, "fail to update profile");
            }

            // Generate Type.
            if (false == GenerateCode_Type())
            {
                throw new ServerException(ServerErrorCode.Generate_Data, "fail to generate type code");
            }

            // Generate Table.
            if (false == GenerateCode_Table())
            {
                throw new ServerException(ServerErrorCode.Generate_Data, "fail to generate table code");
            }

            // Generate Table.
            if (false == GenerateCode_Configuration())
            {
                throw new ServerException(ServerErrorCode.Generate_Data, "fail to generate configuration code");
            }

            // Data code.
            _codeGenerator.CreateFiles(filePath.DataCodePath);

            return new RES_Generate();
        }

        /// <summary>
        /// Generate code for type data.
        /// </summary>
        private bool GenerateCode_Type()
        {
            // Generate Type.
            var types = _typeDataManager.GetSchemaNames();
            foreach (var type in types)
            {
                var schema = _typeDataManager.FindSchemaOrDefault(type);
                if (null == schema)
                {
                    return false;
                }

                var rows = _typeDataManager.GetRowsOrDefault(type);
                if (default == rows)
                {
                    return false;
                }

                _codeGenerator.AddEnum(GENERATE_NAMESPACE, schema.Name, rows);
            }

            return true;
        }

        /// <summary>
        /// Generate code for table data.
        /// </summary>
        private bool GenerateCode_Table()
        {
            // Generate Type.
            var types = _tableDataManager.GetSchemaNames();
            foreach (var type in types)
            {
                var schema = _tableDataManager.FindSchemaOrDefault(type);
                if (null == schema)
                {
                    return false;
                }

                _codeGenerator.AddClass(GENERATE_NAMESPACE, schema.Name, schema.Columns);
            }

            return true;
        }

        /// <summary>
        /// Generate code for configuration data.
        /// </summary>
        private bool GenerateCode_Configuration()
        {
            // Generate Type.
            var types = _configDataManager.GetSchemaNames();
            foreach (var type in types)
            {
                var schema = _configDataManager.FindSchemaOrDefault(type);
                if (null == schema)
                {
                    return false;
                }

                _codeGenerator.AddClass(GENERATE_NAMESPACE, schema.Name, schema.Columns);
            }

            return true;
        }
    }
}
