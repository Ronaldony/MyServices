using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DataDesigner.Core.CodeGenerator
{
    /// <summary>
    /// EnumCodeGenerator.
    /// </summary>
    internal sealed class CodeGenerator
    {
        private readonly ILogger<CodeGenerator> _logger;

        private string _dirPath;

        /// <summary>
        /// Generated types.
        /// </summary>
        private List<Type> _generatedTypes;

        /// <summary>
        /// Type list.
        /// </summary>
        private List<Type> _typeList;

        /// <summary>
        /// CompilationUnitSyntaxs.
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

        public CodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CodeGenerator>>();

            _compilationSyntaxs = new Dictionary<string, CompilationUnitSyntax>();

            _typeTableDic = new Dictionary<string, Type>();

            _typeList = new List<Type>();

            _generatedTypes = new List<Type>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="dirPath">Directory path to generate</param>
        /// <param name="dllFileName">DLL file name to generate</param>
        public void Initialize(string dirPath, string dllFileName)
        {
            _dirPath = dirPath;

            // Create compilation.
            _compilation = CSharpCompilation.Create(
                assemblyName: dllFileName,
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Setup all types.
            _typeList.Clear();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = asm.GetTypes();

                _typeList.AddRange(types);
            }

            // Setup type table.
            _typeTableDic.Clear();
            _typeTableDic.Add("int", typeof(int));
            _typeTableDic.Add("long", typeof(int));
            _typeTableDic.Add("float", typeof(float));
            _typeTableDic.Add("double", typeof(double));
            _typeTableDic.Add("string", typeof(string));
            _typeTableDic.Add("bool", typeof(bool));

            // Initialize.
            _logger.LogInformation("EnumCodeGenerator initialized.");

        }

        /// <summary>
        /// Get generated file full path.
        /// </summary>
        public IEnumerable<Type> GetGeneratedTypes()
        {
            return _generatedTypes;
        }

        /// <summary>
        /// Parse class code data, then generate enum code.
        /// </summary>
        public void AddType(string enumNamespace, string enumName, IEnumerable<EnumMember> enumMembers)
        {
            _logger.LogDebug($"//////////////////////////////////////////////////////////");
            _logger.LogDebug($"[EnumCodeGenerator] Generating start");

            // Get compile unit.
            var compilationSyntax = CreateEnumSyntax(enumNamespace, enumName, enumMembers);

            // Accumulate CompilationUnitSyntax.
            _compilationSyntaxs.Add($"{enumName}.cs", compilationSyntax);

            // Refresh types.
            RefreshTypes(compilationSyntax);

            _logger.LogDebug($"[EnumCodeGenerator] Code generated");
        }

        /// <summary>
        /// Parse class code data, then generate class code.
        /// </summary>
        public bool AddClass(string classNamespace, string className, IEnumerable<ClassMember> classMembers)
        {
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[ClassCodeGenerator] Generating start");

            // Get compile unit.
            var compilationSyntax = CreateClassSyntax(classNamespace, className, classMembers);

            if (null == compilationSyntax)
            {
                _logger.LogError($"[ClassCodeGenerator] Failed to generate code.");
                return false;
            }

            // Accumulate CompilationUnitSyntax.
            _compilationSyntaxs.Add($"{className}.cs", compilationSyntax);

            // Refresh types.
            RefreshTypes(compilationSyntax);

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
        /// Refresh types.
        /// </summary>
        private bool RefreshTypes(CompilationUnitSyntax compilationSyntax)
        {
            _compilation = _compilation.AddSyntaxTrees(compilationSyntax.SyntaxTree);

            using (var ms = new MemoryStream())
            {
                // emit compilation to memory stream.
                var result = _compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    // Load assembly.
                    var assembly = Assembly.Load(ms.ToArray());

                    _generatedTypes = assembly.GetTypes().ToList();
                }
                else
                {
                    // Fail.
                    var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);

                    foreach (var fail in failures)
                    {
                        _logger.LogError($"enum code compilation failed. msg: {fail.ToString()}");
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create enum CodeCompileUnit\.
        /// </summary>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="enumMembers">EnumCodeData list</param>
        private CompilationUnitSyntax CreateEnumSyntax(string enumNamespace, string enumName, IEnumerable<EnumMember> enumMembers)
        {
            // Create namespace.
            var declareNamespace = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.IdentifierName(enumNamespace));

            // Create enum declare.
            var declareEnum = SyntaxFactory
                .EnumDeclaration(enumName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            // Create enum member.
            foreach (var enumMember in enumMembers)
            {
                var memberSyntax = SyntaxFactory
                    .EnumMemberDeclaration(enumMember.Name)         // Name
                    .WithEqualsValue(                               // Value
                        SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(enumMember.Value)
                                )
                            )
                        )
                .WithLeadingTrivia(                                 // 주석 추가
                    SyntaxFactory.Comment($"/// <summary>"), SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment($"/// {enumMember.Comment}"), SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment($"/// </summary>"), SyntaxFactory.CarriageReturnLineFeed
                    );

                // Add member.
                declareEnum = declareEnum.AddMembers(memberSyntax);
            }

            // Add member.
            declareNamespace = declareNamespace.AddMembers(declareEnum);

            return SyntaxFactory.CompilationUnit().AddMembers(declareNamespace).NormalizeWhitespace();
        }


        /// <summary>
        /// Create class CodeCompileUnit.
        /// </summary>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="codeMembers">EnumCodeData list</param>
        private CompilationUnitSyntax CreateClassSyntax(string classNamespace, string className, IEnumerable<ClassMember> classMembers)
        {
            // Create namespace.
            var declareNamespace = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.IdentifierName(classNamespace));

            // Create class declare.
            var declareClass = SyntaxFactory
                .ClassDeclaration(className)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var namespaceList = new HashSet<string>();

            // Create member.
            foreach (var classMember in classMembers)
            {
                var type = FindType(classMember.Type);
                if (null == type)
                {
                    // Not found type.
                    return null;
                }

                // Add type into HashSet.
                if (false == namespaceList.Contains(type.Namespace))
                {
                    namespaceList.Add(type.Namespace);
                }

                // Field.
                var propSyntax = SyntaxFactory
                    .PropertyDeclaration(
                        SyntaxFactory.ParseTypeName(classMember.Type),                          // Type
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
                        SyntaxFactory.Comment($"/// <summary>"), SyntaxFactory.CarriageReturnLineFeed,
                        SyntaxFactory.Comment($"/// {classMember.Comment}"), SyntaxFactory.CarriageReturnLineFeed,
                        SyntaxFactory.Comment($"/// </summary>"), SyntaxFactory.CarriageReturnLineFeed
                    );

                // Add member.
                declareClass = declareClass.AddMembers(propSyntax);
            }


            // Add using.
            foreach (var name in namespaceList)
            {
                // Add namespace for using.
                declareNamespace = declareNamespace
                    .AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(name)
                        ));
            }

            // Add member.
            declareNamespace = declareNamespace.AddMembers(declareClass);

            return SyntaxFactory.CompilationUnit().AddMembers(declareNamespace).NormalizeWhitespace();
        }

        /// <summary>
        /// Get type from name
        /// </summary>
        private Type FindType(string typeName)
        {
            // 1. Find from type table.
            if (true == _typeTableDic.ContainsKey(typeName))
            {
                return _typeTableDic[typeName];
            }

            // 2. Find from generator.
            var types = GetGeneratedTypes();
            if (null != types)
            {
                return types.FirstOrDefault(d => d.Name.Equals(typeName));
            }

            return null;
        }
    }
}
