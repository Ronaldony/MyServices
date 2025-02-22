using PowerArgs;

namespace DataDesignerServer
{
    public class Arguments
    {
        /// <summary>
        /// Port.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("Port")]
        public int Port { get; set; }
    }
}
