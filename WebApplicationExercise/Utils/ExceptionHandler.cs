using System;
using System.Data.Entity.Core;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using WebApplicationExercise.Core;
using WebApplicationExercise.Models;

namespace WebApplicationExercise.Utils
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ExceptionHandler : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(HttpActionExecutedContext context,
            CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                var isCriticalError = false;
                var exceptionLogInfo = CreateException(context);

                if (context.Exception is ArgumentException)
                {
                    exceptionLogInfo.Type = context.Exception as ArgumentNullException != null ?
                         nameof(ArgumentNullException) :
                         context.Exception as ArgumentOutOfRangeException != null ?
                            nameof(ArgumentOutOfRangeException) :
                            nameof(ArgumentException);
                    context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                else
                if (context.Exception is ObjectNotFoundException)
                {
                    exceptionLogInfo.Type = nameof(ObjectNotFoundException);
                    context.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                else
                if (context.Exception is SystemException)
                {
                    exceptionLogInfo.Type = context.Exception as NullReferenceException != null ?
                        nameof(NullReferenceException) :
                        context.Exception as NullReferenceException != null ?
                            nameof(OperationCanceledException) :
                            nameof(SystemException);

                    context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                else
                if (context.Exception is Exception)
                {
                    isCriticalError = true;
                    exceptionLogInfo.Type = nameof(Exception);
                    context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                Logger.Instance.Error(exceptionLogInfo, isCriticalError);
            });
        }

        public ExceptionLog CreateException(HttpActionExecutedContext context)
        {
            return new ExceptionLog
            {
                Message = context.Exception.Message,
                CallStack = context.Exception.ToString(),
                RequestInfo = new ExceptionLog.Request
                {
                    Uri = context.Request.RequestUri.AbsolutePath,
                    Methood = context.Request.Method.Method
                }
            };
        }
    }
}