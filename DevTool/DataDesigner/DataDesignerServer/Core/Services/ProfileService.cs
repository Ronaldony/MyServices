
using System.Collections.Concurrent;

namespace DataDesignerServer.Core.Services
{
	using DataDesignerServer.Core.Data;
	using DataDesignerServer.Core.Services.Interfaces;

	/// <summary>
	/// ProfileService.
	/// </summary>
	public sealed class ProfileService
	{
		// file path
		private const string PROFILE_FOLDER = "Profiles";
		private const string PROFILE_FILE = "profiles.dat";

		private readonly ILogger<ProfileService> _logger;
		private readonly IJsonSerializer _jsonSerializer;

		/// <summary>
		/// Profile dictoinary.
		/// key : Profile name.
		/// value: Profile.
		/// </summary>
		private ConcurrentDictionary<string, Profile> _profileDict;

		private string _profileFolder;

		public ProfileService(
			ILogger<ProfileService> logger,
			IJsonSerializer jsonSerializer)
		{
			_logger = logger;
			_jsonSerializer = jsonSerializer;
		}

		/// <summary>
		/// Initialize.
		/// </summary>
		public void Initialize(string basePath)
		{
			// Load profiles.
			_profileFolder = Path.Combine(basePath, PROFILE_FOLDER);
			_profileDict = LoadProfiles(_profileFolder);
		}

		/// <summary>
		/// Get profile names.
		/// </summary>
		public List<string> GetProfileNames()
		{
			return _profileDict.Keys.ToList();
		}

		/// <summary>
		/// Find profile.
		/// </summary>
		public Profile FindProfileOrDefault(string profileName)
		{
			Profile findProfile;
			if (false == _profileDict.TryGetValue(profileName, out findProfile))
			{
				return default;
			}

			return findProfile;
		}

		/// <summary>
		/// Update profile.
		/// </summary>
		public bool AddOrUpdateProfile(Profile profile)
		{
			if (false == FlushFile())
			{
				return false;
			}

			_profileDict.AddOrUpdate(profile.Name, profile, (key, old) => profile);

			return true;
		}

		/// <summary>
		/// Delete profile.
		/// </summary>
		public bool DeleteProfile(string profileName)
		{
			// check existance.
			if (false == _profileDict.ContainsKey(profileName))
			{
				return false;
			}

			// get value.
			Profile profile;
			if (false == _profileDict.TryGetValue(profileName, out profile))
			{
				return false;
			}

			// remove.
			return _profileDict.Remove(profileName, out profile);
		}

		/// <summary>
		/// Load profiles.
		/// </summary>
		private ConcurrentDictionary<string, Profile> LoadProfiles(string folder)
		{
			// check folder.
			if (false == File.Exists(folder))
			{
				File.Create(folder).Dispose();
			}

			// check profile file.
			var profilePath = Path.Combine(folder, PROFILE_FILE);
			if (File.Exists(profilePath))
			{
				File.Create(profilePath).Dispose();

				return new ConcurrentDictionary<string, Profile>();
			}

			// Load profile file.
			var profileText = File.ReadAllText(profilePath);

			try
			{
				return _jsonSerializer.Deserialize<ConcurrentDictionary<string, Profile>>(profileText);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				// overwrite file.
				File.Create(profilePath).Dispose();

				return new ConcurrentDictionary<string, Profile>();
			}
		}

		/// <summary>
		/// Flush profile file.
		/// </summary>
		private bool FlushFile()
		{
			// check profile file.
			var profilePath = Path.Combine(_profileFolder, PROFILE_FILE);
			if (false == File.Exists(profilePath))
			{
				return false;
			}

			try
			{
				var fileJson = _jsonSerializer.Serialize(_profileDict);
				File.WriteAllText(profilePath, fileJson);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}

			return true;
		}
	}
}
