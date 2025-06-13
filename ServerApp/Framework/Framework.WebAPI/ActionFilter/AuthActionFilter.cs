using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
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
    public sealed class AuthActionFilter : IActionFilter
    {
        /// <summary>
        /// client ip address
        /// </summary>
        public string Address { get; private set; }

        private IGameController _gameController;
        private readonly DBIdentity _dbIdentity;

        public AuthActionFilter(IServiceProvider serviceProvider)
        {
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

            // APIController
            var controller = context.Controller as IGameController;

            // Setup IP Address.
            if (headers.ContainsKey("X-Forwarded-For"))
            {
                Address = headers["X-Forwarded-For"].ToString();
            }

            if (string.IsNullOrEmpty(Address))
            {
                Address = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            // Set controller.
            controller.Uid = uid;
            controller.Pid = dboIdentity.Pid;
            controller.Address = Address;

            _gameController = controller;
        }

        /// <summary>
        /// action executed
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // TODO: Action after excuted.
            var gameController = _gameController as GameController;

            // Upsert controller.
            foreach (var pair in gameController.DataObjectDict)
            {
                pair.Value.UpsertAsync(gameController.Pid);
            }
        }
    }
}
