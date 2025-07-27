using PowerArgs;

namespace Server_Realm
{
    public class Arguments
    {
        /// <summary>
        /// Server name.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Configuration file name.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("Configuration file")]
        public string ConfigFile { get; set; }

        /// <summary>
        /// UDP Port.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("Port")]
        public int Port { get; set; }

        /// <summary>
        /// DataCenter.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("DataCenter")]
        public int DataCenter { get; set; }

        /// <summary>
        /// ServerId.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("ServerId")]
        public int ServerId { get; set; }
    }
}
