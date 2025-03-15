
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Framework.Core.WebAPI.ActionFilter
{
    using Framework.WebAPI.Controllers;

    /// <summary>
    /// auth service action filter
    /// </summary>
    public sealed class AuthActionFilter : IActionFilter
    {
        /// <summary>
        /// header list
        /// </summary>
        private IHeaderDictionary _headers { get; set; }

        /// <summary>
        /// GameContoller.
        /// </summary>
        private GameController GameContoller { get; set; }

        /// <summary>
        /// client ip address
        /// </summary>
        public string Address { get; private set; }

        public AuthActionFilter()
        {
        }

        /// <summary>
        /// context on executing override
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Headers.
            var headers = context.HttpContext.Request.Headers;

            // Setup IP Address.
            if (headers.ContainsKey("X-Forwarded-For"))
            {
                Address = headers["X-Forwarded-For"].ToString();
            }

            if (string.IsNullOrEmpty(Address))
            {
                Address = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            // APIController
            var controller = context.Controller as IGameController;

            controller.Address = Address;
        }

        /// <summary>
        /// action executed
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // TODO: Action after excuted.


        }
    }
}
