
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.Http.Generate;
	using DataDesignerServer.Core.Services;

	/// <summary>
	/// FindProfileController.
	/// </summary>
	public class FindProfileController : Controller
	{
		private readonly ProfileService _profileService;

		public FindProfileController(
			ProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpGet]
		[Route("/generate/find-profile")]
		public RES_FindProfile Process(string profileName)
		{
			// check name
			if (string.IsNullOrEmpty(profileName))
			{
				throw new ServerException(ServerErrorCode.Generate_NoProfileName, "no profile name");
			}

			// find profile
			var findProfile = _profileService.FindProfileOrDefault(profileName);
			if (default == findProfile)
			{
				throw new ServerException(ServerErrorCode.Generate_NotExistProfile, "not exist profile");
			}

			return new RES_FindProfile
			{
				Profile = findProfile,
			};
		}
	}
}
