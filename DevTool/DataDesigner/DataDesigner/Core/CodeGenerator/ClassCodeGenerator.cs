using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace DataDesigner.Core.CodeGenerator
{
    /// <summary>
    /// ClassCodeGenerator.
    /// </summary>
    internal sealed class ClassCodeGenerator
    {
        private readonly ILogger<ClassCodeGenerator> _logger;

        private readonly EnumCodeGenerator _enumCodeGenerator;

        /// <summary>
        /// Assembly list for getting type.
        /// </summary>
        private List<Assembly> _assemblies;

        /// <summary>
        /// Type table dictionary.
        /// key: type name.
        /// value: Type.
        /// </summary>
        private Dictionary<string, Type> _typeTableDic;

        /// <summary>
        /// C# compiler.
        /// </summary>
        private CSharpCompilation _compilation;

        public ClassCodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassCodeGenerator>>();

            _enumCodeGenerator = serviceProvider.GetRequiredService<EnumCodeGenerator>();

            _assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            _typeTableDic = new Dictionary<string, Type>();

            _compilation = CSharpCompilation.Create(
                assemblyName: typeof(ClassCodeGenerator).Name,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        public void Initialize()
        {
            // Setup type table.
            _typeTableDic.Clear();
            _typeTableDic.Add("int", typeof(int));
            _typeTableDic.Add("long", typeof(int));
            _typeTableDic.Add("float", typeof(float));
            _typeTableDic.Add("double", typeof(double));
            _typeTableDic.Add("string", typeof(string));
            _typeTableDic.Add("bool", typeof(bool));

            // Setup C# Complier.
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                _enumCodeGenerator.GetMetadataRef()
            }.ToList();

            _compilation = _compilation.AddReferences(references);
        }

        /// <summary>
        /// Get generated assembly.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetGeneratedTypes()
        {
            var types = new List<Type>();

            using (var ms = new MemoryStream())
            {
                // emit compilation to memory stream.
                var result = _compilation.Emit(ms);

                if (result.Success)
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    types = assembly.GetTypes().ToList();
                }
                else
                {
                    // fail.
                    var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);

                    foreach (var fail in failures)
                    {
                        _logger.LogError($"enum code compilation failed. msg: {fail.ToString()}");
                    }
                }
            }

            return types;
        }

        /// <summary>
        /// Parse class code data, then generate class code.
        /// </summary>
        /// <param name="dirPath">Directory path which file will be generated</param>
        /// <param name="classNamespace">Namespace for class</param>
        /// <param name="className">Class name</param>
        /// <param name="codeDatas">ClassCodeData list</param>
        public bool Generate(string dirPath, string classNamespace, string className, IEnumerable<ClassCodeData> codeDatas)
        {
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[ClassCodeGenerator] Generating start...");

            var compileUnit = GetCompileUnit(classNamespace, className, codeDatas);

            if (null == compileUnit)
            {
                _logger.LogError($"[ClassCodeGenerator] Failed to generate code.");
                return false;
            }

            GenerateCode(compileUnit, dirPath, $"{className}.cs");

            _logger.LogInformation($"[ClassCodeGenerator] Code generated...");

            return true;
        }

        /// <summary>
        /// Get code compile unit for type.
        /// </summary>
        /// <param name="classNamespace">Namespace for class</param>
        /// <param name="className">Class name</param>
        /// <param name="codeDatas">EnumCodeData list</param>
        private CodeCompileUnit GetCompileUnit(string classNamespace, string className, IEnumerable<ClassCodeData> codeDatas)
        {
            // Code namespace.
            var codeNameSpace = new CodeNamespace(classNamespace);

            // Declare enum type.
            var codeType = new CodeTypeDeclaration
            {
                Name = className,
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
            };

            // Add code member.
            foreach (var codeData in codeDatas)
            {
                // Get type.
                var type = GetTypeFromName(codeData.Type);

                // Failed to find type.
                if (null == type)
                {
                    return null;
                }

                var codeMember = new CodeMemberField(type, codeData.Name);

                codeMember.Comments.Add(new CodeCommentStatement
                {
                    Comment = new CodeComment(codeData.Comment)
                });

                codeType.Members.Add(codeMember);
            }

            // Add type into namespace.
            codeNameSpace.Types.Add(codeType);

            // Create CompileUnit.
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(codeNameSpace);

            return compileUnit;
        }

        /// <summary>
        /// Generate code.
        /// </summary>
        /// <param name="compileUnit"></param>
        /// <param name="dirPath"></param>
        /// <param name="outputfileName"></param>
        private bool GenerateCode(CodeCompileUnit compileUnit, string dirPath, string outputfileName)
        {
            // Generate the code with the C# code provider.
            var codeProvider = new CSharpCodeProvider();

            // Create a TextWriter to a StreamWriter to the output file.
            using (StreamWriter sw = new StreamWriter(outputfileName, false))
            {
                var tw = new IndentedTextWriter(sw, "    ");

                var options = new CodeGeneratorOptions();
                options.BlankLinesBetweenMembers = true;

                // Generate source code using the code provider.
                codeProvider.GenerateCodeFromCompileUnit(compileUnit, tw, options);

                // Close the output file.
                tw.Close();

                // Add syntax tree into compilation.
                var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText($"{dirPath}/{outputfileName}"));
                _compilation = _compilation.AddSyntaxTrees(syntaxTree);
            }

            return true;
        }

        /// <summary>
        /// Get type.
        /// </summary>
        private Type GetTypeFromName(string typeName)
        {
            // 1. Find from type table.
            if (true == _typeTableDic.ContainsKey(typeName))
            {
                return _typeTableDic[typeName];
            }

            // 2. Find from enum code generator.
            var generatedTypes = _enumCodeGenerator.GetGeneratedTypes();
            var findType = generatedTypes.FirstOrDefault(d => d.Name.Equals(typeName));
            if (null != findType)
            {
                return findType;
            }

            // 3. Find from Assembly.
            foreach (var asm in _assemblies)
            {
                var type = asm.GetType(typeName, false, true);

                if (null != type)
                {
                    return type;
                }
            }

            return default;
        }
    }
}
