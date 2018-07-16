using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApplicationExercise.Core;

namespace WebApplicationExercise.Utils
{
    public class CustomActionAttribute : ActionFilterAttribute
    {
        private Stopwatch _sw;
        private Guid GUID;

        public override Task OnActionExecutingAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                GUID = Guid.NewGuid();
                Logger.Instance.Information($"Executing request: {actionContext.ControllerContext.Request.Method.Method} " + 
                    $" {actionContext.ControllerContext.Request.RequestUri.AbsolutePath}; for GUID [{GUID}]");
                _sw = Stopwatch.StartNew();
            });
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext,
                                        CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                _sw.Stop();
                if (actionExecutedContext.Response != null)
                {
                    Logger.Instance.Information($"Executed request {actionExecutedContext.Request.Method.Method} {actionExecutedContext.Request.RequestUri.AbsolutePath} " +
                        $" - Time taken: {_sw.Elapsed.TotalMilliseconds}ms; for GUID [{GUID}]");
                }
            });
        }
    }
}