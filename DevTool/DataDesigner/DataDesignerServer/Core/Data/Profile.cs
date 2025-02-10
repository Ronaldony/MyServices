namespace DataDesignerServer.Core.Data
{
	/// <summary>
	/// UserProfile.
	/// </summary>
	public class Profile
	{
		/// <summary>
		/// Name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// File path for server.
		/// </summary>
		public Profile_FilePath ServerFilePath { get; set; }
		
		/// <summary>
		/// File path for client.
		/// </summary>
		public Profile_FilePath ClientFilePath { get; set; }
	}
}
