using System;
using System.Linq;
using System.Net;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using Climax.Web.Http.Extensions;
using System.Net.Http;

namespace Climax.Web.Http.Error
{
    public class ContentNegotiatedExceptionHandler : ExceptionHandler
    {
        private const HttpStatusCode DefaultStatusCode = HttpStatusCode.InternalServerError;
        private const string DefaultErrorMessage =
            "An unexpected error occurred! The error ID will be helpful to debug the problem";


        public override void Handle(ExceptionHandlerContext context)
        {
            Exception exception = GetExceptionForMetadata(context);
            string message = exception != null
                ? GetErrorCodeMessage(exception)
                : DefaultErrorMessage;
            HttpStatusCode statusCode = exception != null
                ? GetErrorCode(exception)
                : DefaultStatusCode;

            var metadata = new ErrorData
            {
                Message = message,
                DateTime = DateTimeOffset.Now,
                RequestUri = context.Request.RequestUri,
                ErrorId = context.Request.GetSafeCorrelationId(),
                Exception = exception
            };

            var response = context.Request.CreateResponse(statusCode, metadata);
            context.Result = new ResponseMessageResult(response);
        }

        private static Exception GetExceptionForMetadata(ExceptionHandlerContext context)
        {
            if (context.RequestContext.IsLocal || context.Request.ShouldIncludeErrorDetail())
            {
                if (context.Exception != null)
                {
                    var exception = context.Exception as AggregateException;
                    return exception != null ? exception.Flatten().InnerExceptions.First() : context.Exception;
                }
            }

            return null;
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }

        protected virtual HttpStatusCode GetErrorCode(Exception exception)
        {
            return DefaultStatusCode;
        }

        protected virtual string GetErrorCodeMessage(Exception exception)
        {
            return DefaultErrorMessage;
        }
    }
}
