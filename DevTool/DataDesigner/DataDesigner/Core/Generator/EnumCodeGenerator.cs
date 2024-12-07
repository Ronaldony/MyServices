using Microsoft.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

namespace DataDesigner.Core.Generator
{
    /// <summary>
    /// EnumParser.
    /// </summary>
    internal sealed class EnumCodeGenerator
    {
        private readonly ILogger<EnumCodeGenerator> _logger;

        public EnumCodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumCodeGenerator>>();
        }

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="typeNamespace">Type name</param>
        /// <param name="typeName">Type name</param>
        /// <param name="typeName">Type name</param>
        public bool Generate(string typeNamespace, string typeName, List<EnumData> enumDatas)
        {
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[EnumParser] Parse datas.");

            // Get compile unit.
            var compileUnit = GetCompileUnit(typeNamespace, typeName, enumDatas);

            // Generate code.
            GenerateCode(compileUnit, AppDomain.CurrentDomain.BaseDirectory, typeName + ".cs");

            return true;
        }

        /// <summary>
        /// Generate code.
        /// </summary>
        private bool GenerateCode(CodeCompileUnit compileUnit, string dirPath, string outputfileName)
        {
            // Generate the code with the C# code provider.
            var codeProvider = new CSharpCodeProvider();

            // Create a TextWriter to a StreamWriter to the output file.
            using (StreamWriter sw = new StreamWriter(outputfileName, false))
            {
                var tw = new IndentedTextWriter(sw, "    ");

                var cgo = new CodeGeneratorOptions();
                cgo.BlankLinesBetweenMembers = true;
                
                // Generate source code using the code provider.
                codeProvider.GenerateCodeFromCompileUnit(compileUnit, tw, cgo);

                // Close the output file.
                tw.Close();
            }

            return true;
        }

        /// <summary>
        /// Parse code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeNamespace"></param>
        /// <param name="typeName"></param>
        /// <param name="typeValue"></param>
        /// <returns></returns>
        private CodeCompileUnit GetCompileUnit(string typeNamespace, string typeName, List<EnumData> enumDatas)
        {
            // Code namespace.
            var codeNameSpace = new CodeNamespace(typeNamespace);

            // Declare enum type.
            var codeType = new CodeTypeDeclaration
            {
                Name = typeName,
                IsEnum = true,
                TypeAttributes = TypeAttributes.Public,
            };

            // Add code member..
            foreach (var enumData in enumDatas)
            {
                var codeMember = new CodeMemberField(typeName, enumData.Key);

                codeMember.InitExpression = new CodePrimitiveExpression(enumData.Value);
                codeMember.Comments.Add(new CodeCommentStatement
                {
                    Comment = new CodeComment(enumData.Comments)
                });

                codeType.Members.Add(codeMember);
            }
            
            // Add type into namespace.
            codeNameSpace.Types.Add(codeType);
            
            // CompileUnit.
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(codeNameSpace);

            return compileUnit;
        }
    }
}
