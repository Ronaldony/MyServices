using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Framework.WebAPI
{
    /// <summary>
    /// ErrorrExceptionFilter.
    /// </summary>
    public class ErrorExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// OnException.
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ServerException)
            {
                var serverException = context.Exception as ServerException;
                if (null == serverException)
                {
                    return;
                }

                // Input error.
                context.HttpContext.Response.Headers.Add("error-code", serverException.ErrorCode.ToString());
                context.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = serverException.ErrorMsg;
            }
        }
    }
}
