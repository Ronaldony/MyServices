namespace DataDesignerServer.Core.Data
{
    /// <summary>
    /// Profile_FilePath.
    /// </summary>
    public class Profile_FilePath
	{
		public Profile_FilePath() { }

		public Profile_FilePath(string target)
		{
			this.Target = target;
			this.ProtocolPath = string.Empty;
			this.DataCodePath = string.Empty;
			this.DatabaseFilePath = string.Empty;
        }

        /// <summary>
        /// Target
        /// </summary>
        public string Target { get; set; }

		/// <summary>
		/// Protocol code.
		/// </summary>
		public string ProtocolPath { get; set; }
		
		/// <summary>
		/// Data code path.
		/// </summary>
		public string DataCodePath { get; set; }

		/// <summary>
		/// Database file path
		/// </summary>
		public string DatabaseFilePath { get; set; }
	}
}
