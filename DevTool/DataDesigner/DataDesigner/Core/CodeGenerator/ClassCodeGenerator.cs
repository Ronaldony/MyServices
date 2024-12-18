using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        private readonly EnumCodeGenerator _enumCodeGenerator;

        private string _dirPath;
        private string _dllFilePath;

        /// <summary>
        /// Type list.
        /// </summary>
        private List<Type> _allTypes;

        /// <summary>
        /// Type table dictionary.
        /// key: file name.
        /// value: CompilationUnitSyntax.
        /// </summary>
        private readonly Dictionary<string, CompilationUnitSyntax> _compilationSyntaxs;

        /// <summary>
        /// Type table dictionary.
        /// key: type name.
        /// value: Type.
        /// </summary>
        private Dictionary<string, Type> _typeTableDic;

        private CSharpCompilation _compilation;

        public ClassCodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassCodeGenerator>>();

            _enumCodeGenerator = serviceProvider.GetRequiredService<EnumCodeGenerator>();

            _typeTableDic = new Dictionary<string, Type>();

            // Initialize type list.
            _allTypes = new List<Type>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = asm.GetTypes();

                _allTypes.AddRange(types);
            }

            _compilationSyntaxs = new Dictionary<string, CompilationUnitSyntax>();
        }

        public void Initialize(string dirPath, string dllFileName)
        {
            // File info.
            _dirPath = dirPath;
            _dllFilePath = Path.Combine(dirPath, dllFileName);

            // Create compilation.
            _compilation = CSharpCompilation.Create(
                assemblyName: dllFileName,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Setup type table.
            _typeTableDic.Clear();
            _typeTableDic.Add("int", typeof(int));
            _typeTableDic.Add("long", typeof(int));
            _typeTableDic.Add("float", typeof(float));
            _typeTableDic.Add("double", typeof(double));
            _typeTableDic.Add("string", typeof(string));
            _typeTableDic.Add("bool", typeof(bool));

            _logger.LogInformation($"ClassCodeGenerator initialized.");
        }

        /// <summary>
        /// Get generated file full path.
        /// </summary>
        public IEnumerable<Type> GetGeneratedTypes()
        {
            var assembly = Assembly.LoadFrom(_dllFilePath);

            return assembly.GetTypes();
        }

        /// <summary>
        /// Parse class code data, then generate class code.
        /// </summary>
        /// <param name="dirPath">Directory path which file will be generated</param>
        /// <param name="classNamespace">Namespace for class</param>
        /// <param name="className">Class name</param>
        /// <param name="classMembers">ClassCodeData list</param>
        public bool AddClass(string classNamespace, string className, IEnumerable<ClassMember> classMembers)
        {
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[ClassCodeGenerator] Generating start");

            // Get compile unit.
            var compilationSyntax = CreateCompilationSyntax(classNamespace, className, classMembers);

            //var compileUnit = GetCompileUnit(classNamespace, className, classMembers);

            if (null == compilationSyntax)
            {
                _logger.LogError($"[ClassCodeGenerator] Failed to generate code.");
                return false;
            }

            // Refresh references for compilation.
            RefreshReferences();

            // Generate DLL.
            GenerateDLL(compilationSyntax);

            // Accumulate CompilationUnitSyntax.
            _compilationSyntaxs.Add($"{className}.cs", compilationSyntax);

            //GenerateCode(compileUnit, $"{className}.cs");

            _logger.LogInformation($"[ClassCodeGenerator] Code generated");

            return true;
        }

        /// <summary>
        /// Create files.
        /// </summary>
        /// <param name="dirPath"></param>
        public void CreateFiles(string dirPath)
        {
            // Create file for generating types.
            foreach (var pair in _compilationSyntaxs)
            {
                var fileName = pair.Key;
                var compilationSyntax = pair.Value;

                var filePath = Path.Combine(_dirPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.WriteAllText(filePath, compilationSyntax.ToFullString());
            }
        }

        /// <summary>
        /// Write the file contains enum.
        /// </summary>
        private bool GenerateDLL(CompilationUnitSyntax compilationSyntax)
        {
            _compilation = _compilation.AddSyntaxTrees(compilationSyntax.SyntaxTree);

            // emit compilation to memory stream.
            var result = _compilation.Emit(_dllFilePath);

            if (!result.Success)
            {
                // Fail.
                var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);

                foreach (var fail in failures)
                {
                    _logger.LogError($"enum code compilation failed. msg: {fail.ToString()}");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Create CodeCompileUnit\.
        /// </summary>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="codeMembers">EnumCodeData list</param>
        private CompilationUnitSyntax CreateCompilationSyntax(string classNamespace, string className, IEnumerable<ClassMember> classMembers)
        {
            // Create namespace.
            var declareNamespace = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.IdentifierName(classNamespace));

            // Create class declare.
            var declareClass = SyntaxFactory
                .ClassDeclaration(className)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            // Create member.
            foreach (var classMember in classMembers)
            {
                var type = GetTypeFromName(classMember.Type);
                if (null == type)
                {
                    // Not found type.
                    return null;
                }

                // Field.
                var propSyntax = SyntaxFactory.PropertyDeclaration(
                        SyntaxFactory.ParseTypeName(classMember.Type),                                 // Type
                        classMember.Name                                                        // Name
                    )
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))                // Public
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),// get
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) // set
                    )
                    .WithLeadingTrivia(                                                         // 주석 추가
                        SyntaxFactory.Comment($"// {classMember.Comment}"),
                        SyntaxFactory.CarriageReturnLineFeed);

                // Add member.
                declareClass = declareClass.AddMembers(propSyntax);
            }

            // Add member.
            declareNamespace = declareNamespace.AddMembers(declareClass);

            return SyntaxFactory.CompilationUnit().AddMembers(declareNamespace).NormalizeWhitespace();
        }

        /// <summary>
        /// Get code compile unit for type.
        /// </summary>
        /// <param name="classNamespace">Namespace for class</param>
        /// <param name="className">Class name</param>
        /// <param name="classMembers">EnumCodeData list</param>
        private CodeCompileUnit GetCompileUnit(string classNamespace, string className, IEnumerable<ClassMember> classMembers)
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
            foreach (var codeData in classMembers)
            {
                // Get type.
                var type = GetTypeFromName(codeData.Type);

                // Failed to find type.
                if (null == type)
                {
                    return null;
                }

                var codeMember = new CodeMemberField(type, codeData.Name);
                codeMember.Attributes = MemberAttributes.Public;

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
        private bool GenerateCode(CodeCompileUnit compileUnit, string outputfileName)
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
                var fileText = File.ReadAllText($"{_dirPath}/{outputfileName}");
                var syntaxTree = CSharpSyntaxTree.ParseText(fileText);

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

            // 2. Find from generator.
            var generatedTypes = _enumCodeGenerator.GetGeneratedTypes();
            var findType = generatedTypes.FirstOrDefault(d => d.Name.Equals(typeName));
            if (null != findType)
            {
                return findType;
            }

            return default;
        }

        /// <summary>
        /// Refresh references.
        /// </summary>
        private void RefreshReferences()
        {
            _compilation = _compilation.RemoveAllReferences();

            _compilation = _compilation.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            _compilation = _compilation.AddReferences(_enumCodeGenerator.GetMetadataReference());
        }
    }
}
