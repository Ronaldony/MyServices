using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DataDesignerServer.Core.Data
{
	using DataDesignerServer.Core.Types;

	/// <summary>
	/// CodeUnitSyntax.
	/// </summary>
	public class CodeUnitSyntax
    {
        /// <summary>
        /// File name.
        /// </summary>
        public string FileName { get; set; }

		/// <summary>
		/// Type_GenerateCode.
		/// </summary>
		public Type_GenerateCode GenCodeType { get; set; }

		/// <summary>
		/// CompilationUnitSyntax.
		/// </summary>
		public CompilationUnitSyntax UnitSyntax {get;set;	}
	}
}
