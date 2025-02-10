
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.Http.Generate;
	using DataDesignerServer.Core.Services;

	/// <summary>
	/// DeleteProfileController.
	/// </summary>
	public class DeleteProfileController : Controller
	{
		private readonly ProfileService _profileService;

		public DeleteProfileController(
			ProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpDelete]
		[Route("/generate/delete-profile")]
		public RES_DeleteProfile Process([FromBody] REQ_DeleteProfile req)
		{
			// check name
			if (string.IsNullOrEmpty(req.Name))
			{
				throw new ServerException(ServerErrorCode.Generate_NoProfileName, "no profile name");
			}

			if (false == _profileService.DeleteProfile(req.Name))
			{
				throw new ServerException(ServerErrorCode.Generate_NoProfileName, "fail to delete profile");
			}

			return new RES_DeleteProfile();
		}
	}
}
