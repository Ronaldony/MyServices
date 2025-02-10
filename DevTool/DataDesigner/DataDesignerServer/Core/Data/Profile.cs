namespace DataDesignerServer.Core.Data
{
    /// <summary>
    /// Profile.
    /// </summary>
    public class Profile
	{
		public Profile() { }

		public Profile(string name) 
		{
			this.Name = name;

			this.FilePathList = new List<Profile_FilePath>();
            this.FilePathList.Add(new Profile_FilePath("Client"));
            this.FilePathList.Add(new Profile_FilePath("Server"));
        }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

		/// <summary>
		/// File path for server.
		/// </summary>
		public List<Profile_FilePath> FilePathList { get; set; }
    }
}
