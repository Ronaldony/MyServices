
namespace DataDesignerServer.Core.Http.Generate
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_Generate.
    /// </summary>
    [HttpAttribute("/generate/data")]
    public class REQ_Generate
    {
        /// <summary>
        /// Target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Profile.
        /// </summary>
        public Profile Profile { get; set; }
    }

    /// <summary>
    /// RES_Generate.
    /// </summary>
    public class RES_Generate
    {
    }
}
