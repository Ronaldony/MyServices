using Microsoft.AspNetCore.Mvc;

namespace DataDesigner.Controllers
{
    using DataDesigner.Controllers.Test;
    using DataDesigner.Core.DataManager;

    /// <summary>
    /// StatusCheckController.
    /// </summary>
    public class StatusCheckController : Controller
    {
        private readonly ILogger<Test_DataManagerController> _logger;

        private readonly ClassManager _classManaer;
        private readonly EnumManager _enumManager;

        public StatusCheckController()
        {
        }

        [HttpGet]
        [Route("")]
        public string Test_Class()
        {
            return "Status OK";
        }
    }
}
