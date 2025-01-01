using Microsoft.AspNetCore.Mvc;

namespace DataDesigner.Controllers
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
