using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DataDesignerServer.Core.Filter
{
    using DataDesignerServer.Core.Common;

    public class ErrorrExceptionFilter : IExceptionFilter
    {
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
                        errorMsg = serverException.Message,
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
