
using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.Http.Generate
{
	/// <summary>
	/// REQ_FindProfile.
	/// </summary>
	[HttpAttribute("/generate/find-profile")]
	public class REQ_FindProfile
	{
	}

	/// <summary>
	/// RES_FindProfile.
	/// </summary>
	public class RES_FindProfile
	{
		/// <summary>
		/// Profile names.
		/// </summary>
		public Profile Profile { get; set; }	
	}
}
