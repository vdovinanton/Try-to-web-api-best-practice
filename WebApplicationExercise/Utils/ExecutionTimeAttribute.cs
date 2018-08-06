using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApplicationExercise.Core;

namespace WebApplicationExercise.Utils
{
    public class ExecutionTimeAttribute : ActionFilterAttribute
    {
        private const string StartTimeKey = "StartTime";
        private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public override Task OnActionExecutingAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                var correlationId = Guid.NewGuid();
                double timestamp = DateTime.Now
                    .ConvertToUnixTimestamp();

                actionContext.Request.Properties.Add(StartTimeKey, timestamp);
                
                _logger.Info($"Executing request: {actionContext.ControllerContext.Request.Method.Method} " +
                    $" {actionContext.ControllerContext.Request.RequestUri.AbsolutePath};");
            });
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext,
                                        CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                double timestamp = default(double);

                if (actionExecutedContext.Request.Properties.ContainsKey(StartTimeKey))
                {
                    double startTimestamp = (double)actionExecutedContext.Request.Properties[StartTimeKey];
                    var startDateTime = startTimestamp
                        .ConvertFromUnixTimestamp();

                    timestamp = (DateTime.Now - startDateTime).TotalMilliseconds;
                }

                if (actionExecutedContext.Response != null)
                {
                    _logger.Info($"Executed request {actionExecutedContext.Request.Method.Method} {actionExecutedContext.Request.RequestUri.AbsolutePath} " +
                        $" - Time taken: {timestamp.ToString()}ms;");
                }
            });
        }
    }
}