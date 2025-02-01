using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DataDesignerServer.Core.Filter
{
    using DataDesignerServer.Core.Common;

    /// <summary>
    /// ErrorrExceptionFilter.
    /// </summary>
    public class ErrorrExceptionFilter : IExceptionFilter
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

                context.Result = new ObjectResult(
                    new
                    {
                        errorMsg = serverException.ErrorMsg,
                        errorCode = serverException.ErrorCode,
                        stackTrace = serverException.StackTrace
                    })
                {
                    StatusCode = StatusCodes.Status200OK,
                };
            }
        }
    }
}
