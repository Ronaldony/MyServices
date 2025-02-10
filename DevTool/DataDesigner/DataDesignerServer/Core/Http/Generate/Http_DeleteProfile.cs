
namespace DataDesignerServer.Core.Http.Generate
{
	using DataDesignerServer.Core.Data;

	/// <summary>
	/// REQ_DeleteProfile.
	/// </summary>
	[HttpAttribute("/generate/delete-profile")]
	public class REQ_DeleteProfile
	{
		/// <summary>
		/// Profile name.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// RES_DeleteProfile.
	/// </summary>
	public class RES_DeleteProfile
	{
	}
}
