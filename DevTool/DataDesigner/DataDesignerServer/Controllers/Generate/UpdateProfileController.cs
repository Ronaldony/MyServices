
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Http.Generate;
    using DataDesignerServer.Core.Services;

    /// <summary>
    /// UpdateProfileController.
    /// </summary>
    public class UpdateProfileController : Controller
	{
		private readonly ProfileService _profileService;

		public UpdateProfileController(
			ProfileService profileService) 
		{
			_profileService = profileService;
		}

		[HttpPatch]
		[Route("/generate/update-profile")]
		public RES_UpdateProfile Process([FromBody] REQ_UpdateProfile req)
		{
			// check name
			if (string.IsNullOrEmpty(req.Profile.Name))
			{
				throw new ServerException(ServerErrorCode.Generate_NoProfileName, "no profile name");
			}

			// Update profile.
			if (false == _profileService.UpdateProfile(req.OriProfileName, req.Profile))
			{
				throw new ServerException(ServerErrorCode.Generate_UpdateProfile, "fail to update profile");
			}

			return new RES_UpdateProfile();
		}
	}
}
