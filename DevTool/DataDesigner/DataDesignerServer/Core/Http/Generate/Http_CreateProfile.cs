
namespace DataDesignerServer.Core.Http.Generate
{
	using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_CreateProfile.
    /// </summary>
    [HttpAttribute("/generate/create-profile")]
	public class REQ_CreateProfile
	{
        /// <summary>
        /// ProfileName.
        /// </summary>
        public string Name { get; set; }
	}

    /// <summary>
    /// RES_CreateProfile.
    /// </summary>
    public class RES_CreateProfile
    {
	}
}
