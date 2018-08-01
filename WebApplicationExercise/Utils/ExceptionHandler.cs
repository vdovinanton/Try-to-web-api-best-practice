using NLog;
using System;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using WebApplicationExercise.Models;

namespace WebApplicationExercise.Utils
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ExceptionHandler : ExceptionFilterAttribute
    {
        private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

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
                        context.Exception as InvalidOperationException != null ?
                            nameof(InvalidOperationException) :
                            nameof(SystemException);

                    context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                else
                if (context.Exception is DataException)
                {
                    exceptionLogInfo.Type = context.Exception as DbUpdateException != null ?
                        nameof(DbUpdateException) :
                        nameof(DataException);

                    context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                else
                if (context.Exception is Exception)
                {
                    isCriticalError = true;
                    exceptionLogInfo.Type = nameof(Exception);
                    context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                //Logger.Instance.Error(exceptionLogInfo, isCriticalError);
                Error(exceptionLogInfo, isCriticalError);
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

        private void Error(ExceptionLog exception, bool isCritical = false)
        {
            var callStack = isCritical ? Environment.NewLine + exception.CallStack : string.Empty;
            var messageTemplate = $"{ exception.RequestInfo.Methood } { exception.RequestInfo.Uri } - { exception.Type } - { exception.Message.Replace('\r', ' ').Replace('\n', ' ') } { callStack }";

            _logger.Error(messageTemplate);
        }
    }
}