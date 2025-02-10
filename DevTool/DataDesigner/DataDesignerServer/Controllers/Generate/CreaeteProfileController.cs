
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Http.Generate;
    using DataDesignerServer.Core.Services;

    /// <summary>
    /// CreateProfileController.
    /// </summary>
    public class CreateProfileController : Controller
	{
		private readonly ProfileService _profileService;

		public CreateProfileController(
			ProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpPost]
		[Route("/generate/create-profile")]
		public RES_CreateProfile Process([FromBody] REQ_CreateProfile req)
		{
			// check name
			if (string.IsNullOrEmpty(req.Name))
			{
				throw new ServerException(ServerErrorCode.Generate_NoProfileName, "no profile name");
			}

			if (false == _profileService.AddProfile(req.Name))
			{
				throw new ServerException(ServerErrorCode.Generate_AddProfile, "fail to add profile");
			}

			return new RES_CreateProfile();
		}
	}
}
