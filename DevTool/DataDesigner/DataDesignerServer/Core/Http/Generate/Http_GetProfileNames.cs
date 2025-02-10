
namespace DataDesignerServer.Core.Http.Generate
{
	/// <summary>
	/// REQ_GetProfileNames.
	/// </summary>
	[HttpAttribute("/generate/get-profile-names")]
	public class REQ_GetProfileNames
	{
	}

	/// <summary>
	/// RES_GetProfileNames.
	/// </summary>
	public class RES_GetProfileNames
	{
		/// <summary>
		/// Profile names.
		/// </summary>
		public List<string> ProfileNames { get; set; }
	}
}
