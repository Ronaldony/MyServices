using PowerArgs;

namespace Server_PlayHub
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
        /// Port for HuntZone.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("PortHuntZone")]
        public int PortHuntZone { get; set; }

        /// <summary>
        /// Port for PlayHub.
        /// </summary>
        [ArgRequired(PromptIfMissing = true), ArgDescription("PortPlayHub")]
        public int PortPlayHub { get; set; }

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
