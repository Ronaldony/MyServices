
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.Http.Generate;
	using DataDesignerServer.Core.Services;

	/// <summary>
	/// GetProfileNamesController.
	/// </summary>
	public class GetProfileNamesController : Controller
	{
		private readonly ProfileService _profileService;

		public GetProfileNamesController(
			ProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpGet]
		[Route("/generate/get-profile-names")]
		public RES_GetProfileNames Process(string profileName)
		{
			// check name
			if (string.IsNullOrEmpty(profileName))
			{
				throw new ServerException(ServerErrorCode.Generate_NoProfileName, "no profile name");
			}

			return new RES_GetProfileNames
			{
				ProfileNames = _profileService.GetProfileNames(),
			};
		}
	}
}
