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
        private const string CorrelationIdKey = "CorrelationId";

        public override Task OnActionExecutingAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                var correlationId = Guid.NewGuid();
                double timestamp = DateTime.Now
                    .ConvertToUnixTimestamp();

                actionContext.Request.Properties.Add(StartTimeKey, timestamp);
                actionContext.Request.Properties.Add(CorrelationIdKey, correlationId);

                Logger.Instance.Information($"Executing request: {actionContext.ControllerContext.Request.Method.Method} " + 
                    $" {actionContext.ControllerContext.Request.RequestUri.AbsolutePath}; for CorrelationId: [{correlationId}]");
            });
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext,
                                        CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                double timestamp = default(double);
                string correlectionId = actionExecutedContext.Request.Properties.ContainsKey(CorrelationIdKey) ?
                    actionExecutedContext.Request.Properties[CorrelationIdKey].ToString() :
                    string.Empty;

                if (actionExecutedContext.Request.Properties.ContainsKey(StartTimeKey))
                {
                    double startTimestamp = (double)actionExecutedContext.Request.Properties[StartTimeKey];
                    var startDateTime = startTimestamp
                        .ConvertFromUnixTimestamp();

                    timestamp = (DateTime.Now - startDateTime).TotalMilliseconds;
                }

                if (actionExecutedContext.Response != null)
                {
                    Logger.Instance.Information($"Executed request {actionExecutedContext.Request.Method.Method} {actionExecutedContext.Request.RequestUri.AbsolutePath} " +
                        $" - Time taken: {timestamp.ToString()}ms; for CorrelationId [{correlectionId}]");
                }
            });
        }
    }
}