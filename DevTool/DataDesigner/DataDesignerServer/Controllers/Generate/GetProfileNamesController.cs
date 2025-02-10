
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Generate
{
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
		public RES_GetProfileNames Process()
		{
			return new RES_GetProfileNames
			{
				ProfileNames = _profileService.GetProfileNames(),
			};
		}
	}
}
