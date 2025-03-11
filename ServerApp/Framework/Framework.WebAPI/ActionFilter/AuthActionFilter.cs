
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Framework.Core.WebAPI.ActionFilter
{
	using Framework.WebAPI;
	using Framework.WebAPI.Attributes;
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
        /// client ip address
        /// </summary>
        public string Address { get; private set; }

        private readonly ILogger<AuthActionFilter> _logger;
        //private readonly GameSessionService _gameSessionService;

        public AuthActionFilter(ILogger<AuthActionFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// context on executing override
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Headers.
            _headers = context.HttpContext.Request.Headers;

            // Setup IP Address.
            if (_headers.ContainsKey("X-Forwarded-For"))
            {
                Address = _headers["X-Forwarded-For"].ToString();
            }

            if (string.IsNullOrEmpty(Address))
            {
                Address = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            // APIController
            var controller = context.Controller as IGameController;

            if (false == context.ActionArguments.ContainsKey("req"))
            {
                throw new ServerException(1, "Not exist request");
            }

            var req = context.ActionArguments["req"];
            var reqTypeInfo = req.GetType().GetTypeInfo();

            bool isAuthCheck = false;
            var ignoreAuthAttr = controller.GetType().GetCustomAttribute<IgnoreAuthAttribute>();
            if (null == ignoreAuthAttr)
            {
                isAuthCheck = true;
            }

            // auth information exist protocol
            if (isAuthCheck)
            {
                var uid = _headers["uid"].ToString();

                if (false == string.IsNullOrEmpty(uid))
                {
                    // Exception.
                }

                if (false == _headers.ContainsKey("act"))
                {
                    // Exception.
                    throw new ServerException(1, "not exist access token");
				}

				//var accessTokenString = _headers["act"].ToString();
				//var accessToken = _gameSessionService.ResolveAccessTokenFromString(accessTokenString);

                // TODO: Access token 정보 비교.
			}
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
