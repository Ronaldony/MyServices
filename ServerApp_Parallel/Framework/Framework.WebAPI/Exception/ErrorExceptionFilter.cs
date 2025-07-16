using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

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
                context.HttpContext.Response.Headers.Append(new KeyValuePair<string, StringValues>("error-code", serverException.ErrorCode.ToString()));
                context.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = serverException.ErrorMsg;
            }
        }
    }
}
