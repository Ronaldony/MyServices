using Microsoft.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

namespace DataDesigner.Core.CodeGenerator
{
    /// <summary>
    /// ClassCodeGenerator.
    /// </summary>
    internal sealed class ClassCodeGenerator
    {
        private readonly ILogger<ClassCodeGenerator> _logger;

        private List<Assembly> _assemblies;

        private Dictionary<string, Type> _typeTableDic;

        public ClassCodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassCodeGenerator>>();

            _assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            _typeTableDic = new Dictionary<string, Type>();
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
                _logger.LogError($"[EnumCodeGenerator] Failed to generate code.");
                return false;
            }

            GenerateCode(compileUnit, dirPath, $"{className}.cs");

            _logger.LogInformation($"[EnumCodeGenerator] Code generated...");

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
            }

            return true;
        }

        /// <summary>
        /// Get type.
        /// </summary>
        private Type GetTypeFromName(string typeName)
        {
            // Find from type table.
            if (true == _typeTableDic.ContainsKey(typeName))
            {
                return _typeTableDic[typeName];
            }

            // Find from Assembly.
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
