using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Framework.Core.WebAPI.ActionFilter
{
    using Framework.Core.Database.DAO;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using GameCore;

    /// <summary>
    /// auth service action filter
    /// </summary>
    public sealed class AuthActionFilter : IAsyncActionFilter
    {
        /// <summary>
        /// client ip address
        /// </summary>
        public string Address { get; private set; }

        private readonly ILogger<AuthActionFilter> _logger;

        private IGameController _gameController;
        private readonly DBIdentity _dbIdentity;

        public AuthActionFilter(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<AuthActionFilter>>();

            _dbIdentity = serviceProvider.GetRequiredService<DBIdentity>();
        }

        /// <summary>
        /// context on executing override
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Headers.
            var headers = context.HttpContext.Request.Headers;

            // Check identity.
            if (false == headers.ContainsKey("uid"))
            {
                throw new ServerException(GameErrorCode.AuthFilter_EmptyUid, $"empty uid in http header.");
            }

            if (false == headers.ContainsKey("accesstoken"))
            {
                throw new ServerException(GameErrorCode.AuthFilter_EmptyToken, $"empty access token in http header.");
            }

            // User 정보.
            var accessToken = headers["accesstoken"].ToString();
            var uid = headers["uid"].ToString();
            var dboIdentity = _dbIdentity.SelectOrDefault(uid);

            if (null == dboIdentity)
            {
                throw new ServerException(GameErrorCode.AuthFilter_NotExistUser, $"not exist user. uid: {uid}");
            }

            // Token 검사.
            var nowTime = TimeUtil.Now;

            if (TimeUtil.IsPastTime(dboIdentity.TokenExpireTime, nowTime))
            {
                throw new ServerException(GameErrorCode.AuthFilter_ExpiredToken, $"expire accesstoken.");
            }

            if (false == dboIdentity.Token.Equals(accessToken))
            {
                throw new ServerException(GameErrorCode.AuthFilter_NotmatchToken, $"not match accesstoken.");
            }

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

            // Set controller.
            controller.Uid = uid;
            controller.Pid = dboIdentity.Pid;
            controller.Address = Address;

            _gameController = controller;
        }

        /// <summary>
        /// action executed
        /// </summary>
        public async Task OnActionExecutedAsync()
        {
            // TODO: Action after excuted.
            var gameController = _gameController as GameController;
            var pid = gameController.Pid;

            // Upsert controller.
            foreach (var pair in gameController.DataObjectDict)
            {
                for (int tryCount = 0; tryCount < 5; tryCount++)
                {
                    if (await pair.Value.UpsertAsync(pid))
                    {
                        // Success.
                        break;
                    }

                    _logger.LogError($"======================================================");
                    _logger.LogError($"======================================================");
                    _logger.LogError($"failed to upsert data object!!! try count - {tryCount}, pid: {pid}.");
                    _logger.LogError($"DataObject type: {pair.Key}");
                }
            }
        }

        /// <summary>
        /// OnActionExecution async.
        /// </summary>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Before action.
            OnActionExecuting(context);

            await next();

            // After action.
            await OnActionExecutedAsync();
        }
    }
}
