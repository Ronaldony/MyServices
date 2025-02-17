using PowerArgs;

namespace ServerEngine.Test
{
    public class Arguments
    {
        /// <summary>
        /// Server name.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("Server")]
        public string Server { get; set; }

        /// <summary>
        /// Configuration file name.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("Configuration file")]
        public string ConfigFile { get; set; }

        [ArgRequired(PromptIfMissing = true), ArgDescription("Port")]
        public int Port { get; set; }
    }
}
