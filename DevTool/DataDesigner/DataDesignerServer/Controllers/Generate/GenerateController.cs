using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        private const string PROTOCOL_PROJECT = "GameCore.csproj";
        private const string PROTOCOL_FOLDER = "DataDesignerProtocol";

        private readonly ClassDataManager _classDataManager;
        private readonly TypeDataManager _typeDataManager;

        private readonly CodeGenerator _codeGenerator;
        private readonly DBGenerator _dbGenerator;

        // Name of files and folders to exclude from generating protocol.
        private string[] _exclueFiles = [".sln", ".csproj"];
        private string[] _exclueFolders = [".vs", "bin", "obj", "CodeGen"];

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

            CreateFolders(filePath);

            // Build protocol.
            BuildAndCreateProtocolFiles(filePath.ProtocolPath);

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

            // Create code.
            _codeGenerator.CreateFiles(filePath.DataCodePath);

            // Create protocols.

            return new RES_Generate();
        }

        /// <summary>
        /// Build protocol project.
        /// </summary>
        private void BuildAndCreateProtocolFiles(string dstProtocolPath)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var protocolFolder = Path.Combine(currentDir, PROTOCOL_FOLDER);

            // Compile.
            if (Directory.Exists(protocolFolder))
            {
                // Protocol exist in Current folder.
                if (false == CompileProejct(protocolFolder))
                {
                    throw new ServerException(ServerErrorCode.Generate_ProtocolCompile, "failed to compile protocol project!");
                }

                CreateProtocolFiles(protocolFolder, dstProtocolPath);
            }
            else
            {
                // Protocol exist in Current folder.
                var parentDir = Directory.GetParent(currentDir).FullName;
                protocolFolder = Path.Combine(parentDir, PROTOCOL_FOLDER);

                if (Directory.Exists(protocolFolder))
                {
                    if (false == CompileProejct(protocolFolder))
                    {
                        throw new ServerException(ServerErrorCode.Generate_ProtocolCompile, "failed to compile protocol project!");
                    }

                    CreateProtocolFiles(protocolFolder, dstProtocolPath);
                }
                else
                {
                    throw new ServerException(ServerErrorCode.Generate_NotExistProtocol, "not exist protocol project");
                }
            }
        }

        /// <summary>
        /// Compile project.
        /// </summary>
        private bool CompileProejct(string directory)
        {
            string projectPath = Path.Combine(directory, PROTOCOL_PROJECT);

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build \"{projectPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (0 != process.ExitCode)
                {
                    // Error.
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create protocol files.
        /// </summary>
        private void CreateProtocolFiles(string srcDirectory, string dstProtocolPath)
        {
            // Files.
            var files = Directory.GetFiles(srcDirectory).Where(d => default == _exclueFiles.FirstOrDefault(e => d.Contains(e)));
            
            foreach (var file in files)
            {
                var newFile = Path.Combine(dstProtocolPath, Path.GetFileName(file));

                try
                {
                    System.IO.File.Create(newFile).Dispose();
                    System.IO.File.Copy(file, newFile, true);
                }
                catch (Exception ex)
                {
                    throw new ServerException(ServerErrorCode.Generate_ProtocolCompile, ex.Message);
                }
            }

            // Directories.
            var directories = Directory.GetDirectories(srcDirectory).Where(d => default == _exclueFolders.FirstOrDefault(e => d.Contains(e)));

            foreach (var directory in directories)
            {
                var dirName = Path.GetFileName(directory);
                var dstDirName = Path.Combine(dstProtocolPath, dirName);

                CopyDirectory(directory, dstDirName);
            }
        }

        /// <summary>
        /// CopyDirectory.
        /// </summary>
        private void CopyDirectory(string curDirectory, string dstProtocolPath)
        {
            Directory.CreateDirectory(dstProtocolPath);

            foreach (var file in Directory.GetFiles(curDirectory))
            {
                var newFile = Path.Combine(dstProtocolPath, Path.GetFileName(file));

                try
                {
                    System.IO.File.Create(newFile).Dispose();
                    System.IO.File.Copy(file, newFile, true);
                }
                catch (Exception ex)
                {
                    throw new ServerException(ServerErrorCode.Generate_ProtocolCompile, ex.Message);
                }
            }

            foreach (var directory in Directory.GetDirectories(curDirectory))
            {
                var newDir = Path.Combine(dstProtocolPath, Path.GetFileName(directory));
                CopyDirectory(directory, newDir);
            }
        }

        /// <summary>
        /// Create files.
        /// </summary>
        private void CreateFolders(Profile_FilePath filePath)
        {
            // Protocol path.
            if (Directory.Exists(filePath.ProtocolPath))
            {
                Directory.Delete(filePath.ProtocolPath, true);
            }

            Directory.CreateDirectory(filePath.ProtocolPath);

            // Data code path.
            if (Directory.Exists(filePath.DataCodePath))
            {
                Directory.Delete(filePath.DataCodePath, true);
            }

             Directory.CreateDirectory(filePath.DataCodePath);

            // Database path
            if (Directory.Exists(filePath.DatabaseFilePath))
            {
                Directory.Delete(filePath.DatabaseFilePath, true);
            }

            Directory.CreateDirectory(filePath.DatabaseFilePath);
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

                try
                {
                    _dbGenerator.GenerateClass(dbFilePath, type, schema, rows);
                }
                catch (Exception ex)
                {
                    throw new ServerException(ServerErrorCode.Generate_NotCreateDatbase, ex.Message);
                }
            }

            return true;
        }
    }
}
