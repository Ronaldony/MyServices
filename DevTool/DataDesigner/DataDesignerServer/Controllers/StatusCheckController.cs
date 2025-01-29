using DataDesignerServer.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers
{
    /// <summary>
    /// StatusCheckController.
    /// </summary>
    public class StatusCheckController : Controller
    {
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
