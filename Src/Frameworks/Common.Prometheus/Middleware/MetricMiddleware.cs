using Common.Observability.Helper;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;


namespace Common.Observability.Middleware
{
    public class MetricMiddleware
    {
        private readonly RequestDelegate _request;

        public MetricMiddleware(RequestDelegate request)
        {
            _request = request ?? throw new Exception(string.Format("Context is empty; {0}", nameof(request)));
        }

        public async Task Invoke(HttpContext context, MetricReporter reporter)
        {
            if (context.Request.Path.Value == "/metrics")
            {
                await _request.Invoke(context);
                return;
            }
            var sw = Stopwatch.StartNew();

            try
            {
                await _request.Invoke(context);
            }
            finally
            {
                sw.Stop();
                reporter.RegisterRequest();
                reporter.RegisterResponseTime(context.Response.StatusCode, context.Request.Method, sw.Elapsed);
            }
        }
    }
}
