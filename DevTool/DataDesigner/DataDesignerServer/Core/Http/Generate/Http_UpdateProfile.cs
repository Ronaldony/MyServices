
namespace DataDesignerServer.Core.Http.Generate
{
	using DataDesignerServer.Core.Data;

	/// <summary>
	/// REQ_UpdateProfile.
	/// </summary>
	[HttpAttribute("/generate/update-profile")]
	public class REQ_UpdateProfile
	{
		public Profile Profile { get; set; }
	}

	/// <summary>
	/// RES_UpdateProfile.
	/// </summary>
	public class RES_UpdateProfile
	{
	}
}
